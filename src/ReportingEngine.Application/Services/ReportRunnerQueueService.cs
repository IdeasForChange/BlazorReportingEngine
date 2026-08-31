using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.Configuration;
using Smbc.Risk.ReportingEngine.Application.Interfaces;
using Smbc.Risk.ReportingEngine.Domain.Entities;
using Smbc.Risk.ReportingEngine.Domain.Repositories;
using Smbc.Risk.ReportingEngine.Domain.Services;
using Smbc.Risk.ReportingEngine.Domain.Shared.Enums;
using System.Text.Json;

namespace Smbc.Risk.ReportingEngine.Application.Services;

public class ReportRunnerQueueService(
    IReportMasterRepository reportMasterRepository,
        IReportRunnerQueueRepository repository,
        IDynamicQueryExecutor dynamicQueryExecutor,
        ReportJobChannel jobChannel,
        IConfiguration configuration) : IReportRunnerQueueService
{
    public async Task<List<long>> ClaimPendingJobIdsAsync(int batchSize, CancellationToken cancellationToken)
    {
        return await repository.ClaimPendingJobIdsAsync(batchSize, cancellationToken);
    }

    public async Task<long> EnqueueJobAsync(long reportMasterId, string parameterValuesJson, string requestedBy, CancellationToken cancellationToken)
    {
        // Entity creation handled via DB Context in production flow
        var queueItem = new ReportRunnerQueue
        {
            ReportMasterId = reportMasterId,
            Status = QueueStatus.Pending, // Pending
            ParameterValuesJson = parameterValuesJson,
            ProgressPercentage = 0,
            CreatedBy = requestedBy,
            CreatedAtUtc = DateTime.UtcNow
        };

        // Save via EF Repository...
        // After save, notify background processor
        await jobChannel.Writer.WriteAsync(queueItem.Id, cancellationToken);
        return queueItem.Id;
    }

    public async Task ProcessQueueItemAsync(long jobId, CancellationToken cancellationToken)
    {
        var job = await repository.GetByIdAsync(jobId, cancellationToken);
        if (job == null || job.Status != QueueStatus.Processing) return;

        string? generatedTempPath = null;

        try
        {
            await repository.UpdateJobStatusAsync(jobId, status: QueueStatus.Processing, progress: 10, cancellationToken: cancellationToken); // Processing

            var reportMaster = await reportMasterRepository.GetByIdAsync(job.ReportMasterId, cancellationToken) ?? throw new InvalidOperationException("Report master configuration not found.");
            var activeTemplate = reportMaster.ReportTemplates.FirstOrDefault(t => t.IsActive) ?? throw new InvalidOperationException("No active template found for report.");
            cancellationToken.ThrowIfCancellationRequested();

            // Prepare Paths
            string templateFullPath = activeTemplate.TemplatePath;

            // Ensure output directory exists
            string outputDirectory = reportMaster.ReportDirectory;
            Directory.CreateDirectory(outputDirectory);

            // Generate unique output file name
            string outputFileName = $"{reportMaster.ReportNamePattern}_{jobId}_{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx";
            generatedTempPath = Path.Combine(outputDirectory, outputFileName);

            // Copy template to destination target
            File.Copy(templateFullPath, generatedTempPath, overwrite: true);

            await repository.UpdateJobStatusAsync(jobId, status: QueueStatus.Processing, progress: 30, cancellationToken: cancellationToken);

            // Parse parameter dictionary
            var parameters = string.IsNullOrWhiteSpace(job.ParameterValuesJson)
                ? new Dictionary<string, string>()
                : JsonSerializer.Deserialize<Dictionary<string, string>>(job.ParameterValuesJson) ?? new();

            // Open Excel via ClosedXML
            using (var workbook = new XLWorkbook(generatedTempPath))
            {
                int totalMetrics = activeTemplate.ReportMetrics.Count;
                int processedMetrics = 0;

                foreach (var metric in activeTemplate.ReportMetrics.Where(m => m.IsActive))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    // Replace Parameter placeholders (@ParamName) in SQL
                    string finalSql = metric.SqlQuery;
                    foreach (var param in parameters)
                    {
                        finalSql = finalSql.Replace($"@{param.Key}", param.Value.Replace("'", "''"));
                    }

                    // Execute Dynamic Query via EF Connection / DbConnection
                    var dataTable = await dynamicQueryExecutor.ExecuteQueryAsync(metric.DatabaseConnectionId, finalSql, metric.MaxRows, cancellationToken);

                    // ClosedXML - Populate Named Range
                    if (workbook.DefinedNames.TryGetValue(metric.NamedRange, out var namedRange))
                    {
                        // 1. Locate your named range or target cell (e.g., Cell A2)
                        var targetCell = namedRange.Ranges.First().FirstCell();
                        int rowCount = dataTable.Rows.Count;

                        // 2. Insert blank rows above your formula row to push it down
                        // True = shift existing cells down and copy formatting from the row above
                        targetCell.WorksheetRow().InsertRowsBelow(rowCount);

                        // 3. Populate data into the newly created space
                        targetCell.InsertData(dataTable);
                    }

                    processedMetrics++;
                    int currentProgress = 30 + (int)((processedMetrics / (double)totalMetrics) * 60);
                    await repository.UpdateJobStatusAsync(jobId, status: QueueStatus.Processing, progress: currentProgress, cancellationToken: cancellationToken);
                }

                workbook.Save();
            }

            await repository.UpdateJobStatusAsync(jobId, status: QueueStatus.Completed, progress: 100, outputFilePath: generatedTempPath, cancellationToken: cancellationToken); // Success
        }
        catch (OperationCanceledException)
        {
            // Clean up resources if cancelled
            if (!string.IsNullOrEmpty(generatedTempPath) && File.Exists(generatedTempPath))
            {
                File.Delete(generatedTempPath);
            }
            await repository.UpdateJobStatusAsync(jobId, status: QueueStatus.Cancelled, progress: 0, errorMessage: "Job execution was cancelled.", cancellationToken: CancellationToken.None);
        }
        catch (Exception ex)
        {
            if (!string.IsNullOrEmpty(generatedTempPath) && File.Exists(generatedTempPath))
            {
                File.Delete(generatedTempPath);
            }
            await repository.UpdateJobStatusAsync(jobId, status: QueueStatus.Failed, progress: 0, errorMessage: ex.Message, cancellationToken: CancellationToken.None); // Failed
        }
    }

    public Task CancelJobAsync(long jobId, CancellationToken cancellationToken)
    {
        // Cancellation tokens managed per job in job manager engine
        return Task.CompletedTask;
    }

    //private async Task<DataTable> ExecuteMetricQueryAsync(long? connectionId, string sql, int? maxRows, CancellationToken cancellationToken)
    //{
    //    DataTable dt = new DataTable();
    //    string connectionString = _configuration.GetConnectionString("DefaultConnection")!;

    //    if (connectionId.HasValue)
    //    {
    //        var dbConn = await _repository.GetDatabaseConnectionAsync(connectionId.Value, cancellationToken);
    //        if (dbConn != null)
    //        {
    //            connectionString = $"Server={dbConn.ServerHost},{dbConn.Port};Database={dbConn.DatabaseName};User Id={dbConn.UserId};Password={dbConn.Password};Timeout={dbConn.TimeoutSeconds};TrustServerCertificate=True;";
    //        }
    //    }

    //    using (var conn = new Microsoft.Data.SqlClient.SqlConnection(connectionString))
    //    {
    //        await conn.OpenAsync(cancellationToken);
    //        using (var cmd = conn.CreateCommand())
    //        {
    //            cmd.CommandText = maxRows.HasValue ? $"SELECT TOP ({maxRows.Value}) * FROM ({sql}) AS MetricSubQuery" : sql;
    //            using (var reader = await cmd.ExecuteReaderAsync(cancellationToken))
    //            {
    //                dt.Load(reader);
    //            }
    //        }
    //    }
    //    return dt;
    //}
}
