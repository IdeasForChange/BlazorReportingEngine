using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NodaTime.Text;
using Smbc.Risk.Core.Application.Services;
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
        IConfiguration configuration,
        IExcelParserService excelParserService,
        ILogger<ReportRunnerQueueService> logger) : IReportRunnerQueueService
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
        logger.LogInformation($"Querying details for Job Id '{jobId}' from teh database.");

        var job = await repository.GetByIdAsync(jobId, cancellationToken);
        if (job == null || job.Status != QueueStatus.Processing)
        {
            logger.LogInformation($"Unable to find the job for Job Id '{jobId}'. RETURNING!");
            return;
        }

        string? generatedTempPath = null;

        try
        {
            await repository.UpdateJobStatusAsync(jobId, status: QueueStatus.Processing, progress: 10, cancellationToken: cancellationToken); // Processing
            logger.LogInformation($"Updated Job Id '{jobId}' with 10% progress.");

            var reportMaster = await reportMasterRepository.GetByIdAsync(job.ReportMasterId, cancellationToken) ?? throw new InvalidOperationException("Report master configuration not found.");
            var activeTemplate = reportMaster.ReportTemplates.FirstOrDefault(t => t.IsActive) ?? throw new InvalidOperationException("No active template found for report.");
            cancellationToken.ThrowIfCancellationRequested();

            // Prepare Paths
            string templateFullPath = Path.Combine(activeTemplate.TemplatePath, activeTemplate.UploadedFileName);
            logger.LogInformation($"Found the full template Path for Job Id '{jobId}' : {activeTemplate.TemplatePath}");

            // Ensure output directory exists
            string outputDirectory = reportMaster.ReportDirectory;
            Directory.CreateDirectory(outputDirectory);
            logger.LogInformation($"Making sure the Report Directory exists for Job Id '{jobId}' : {outputDirectory}");

            // Generate unique output file name
            string outputFileName = ReplaceToken($"{reportMaster.ReportNamePattern}.xlsx");
            generatedTempPath = Path.Combine(outputDirectory, outputFileName);
            logger.LogInformation($"Report will be generated here : {generatedTempPath}");

            // Copy template to destination target
            File.Copy(templateFullPath, generatedTempPath, overwrite: true);

            await repository.UpdateJobStatusAsync(jobId, status: QueueStatus.Processing, progress: 30, cancellationToken: cancellationToken);
            logger.LogInformation($"Updated Job Id '{jobId}' with 30% progress.");

            // Parse parameter dictionary
            var parameters = string.IsNullOrWhiteSpace(job.ParameterValuesJson)
                ? new Dictionary<string, string>()
                : JsonSerializer.Deserialize<Dictionary<string, string>>(job.ParameterValuesJson) ?? new();

            // Open Excel via ClosedXML
            using (var workbook = new XLWorkbook(generatedTempPath))
            {
                // Get all the Workbook DefinedNames
                var definedNames = workbook.DefinedNames.ToList();

                // Also add all the worksheet name range
                definedNames.AddRange(workbook.Worksheets.SelectMany(ws => ws.DefinedNames).ToList());

                int totalMetrics = activeTemplate.ReportMetrics.Count;
                int processedMetrics = 0;

                foreach (var metric in activeTemplate.ReportMetrics.Where(m => m.IsActive))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if(metric.DatabaseConnectionId is null)
                    {
                        logger.LogWarning($"Metric SQL for {metric.NamedRange} is ignored as NO database connection is specified.");
                        continue;
                    }

                    // Replace Parameter placeholders (@ParamName) in SQL
                    string finalSql = metric.SqlQuery;
                    foreach (var param in parameters)
                    {
                        finalSql = finalSql.Replace($"@{param.Key}", param.Value.Replace("'", "''"));
                    }

                    logger.LogInformation($"Executing SQL: {finalSql}, Max Rows: {metric.MaxRows} against Database Connection: {metric.DatabaseConnectionId}.");

                    // Execute Dynamic Query via EF Connection / DbConnection
                    var dataTable = await dynamicQueryExecutor.ExecuteQueryAsync(metric.DatabaseConnectionId, finalSql, metric.MaxRows, cancellationToken);

                    var namedRange = definedNames.Where(p => p.Name.Equals(metric.NamedRange)).FirstOrDefault();
                    if (namedRange != null)
                    {
                        // 1. Locate the named range or target cell (e.g., Cell A2)
                        var targetCell = namedRange.Ranges.First().FirstCell();
                        int rowCount = dataTable.Rows.Count;

                        // 2. Is the number of result is one, just INSERT the data in the cell
                        if (rowCount > 1)
                        {
                            targetCell.WorksheetRow().InsertRowsBelow(rowCount);
                        }

                        // 3. Populate data into the newly created space
                        targetCell.InsertData(dataTable);

                        logger.LogInformation($"INSERTED: {rowCount} number of rows in the spreadsheet for named range: {metric.NamedRange}.");
                    }

                    processedMetrics++;
                    int currentProgress = 30 + (int)((processedMetrics / (double)totalMetrics) * 60);
                    await repository.UpdateJobStatusAsync(jobId, status: QueueStatus.Processing, progress: currentProgress, cancellationToken: cancellationToken);
                }

                workbook.Save();
            }

            await repository.UpdateJobStatusAsync(jobId, status: QueueStatus.Completed, progress: 100, outputFilePath: generatedTempPath, cancellationToken: cancellationToken); // Success
            logger.LogInformation($"Job completed and report saved successfully '{jobId}' : {generatedTempPath}");

        }
        catch (OperationCanceledException)
        {
            // Clean up resources if cancelled
            if (!string.IsNullOrEmpty(generatedTempPath) && File.Exists(generatedTempPath))
            {
                File.Delete(generatedTempPath);
            }
            await repository.UpdateJobStatusAsync(jobId, status: QueueStatus.Cancelled, progress: 0, errorMessage: "Job execution was cancelled.", cancellationToken: CancellationToken.None);
            logger.LogInformation($"Job was cancelled. Deleted the temporary file for '{jobId}'");
        }
        catch (Exception ex)
        {
            if (!string.IsNullOrEmpty(generatedTempPath) && File.Exists(generatedTempPath))
            {
                File.Delete(generatedTempPath);
            }
            await repository.UpdateJobStatusAsync(jobId, status: QueueStatus.Failed, progress: 0, errorMessage: ex.Message, cancellationToken: CancellationToken.None); // Failed
            logger.LogInformation($"Job failed to execute. Please see the logs for details: {ex.Message}");
        }
    }

    public Task CancelJobAsync(long jobId, CancellationToken cancellationToken)
    {
        // Cancellation tokens managed per job in job manager engine
        return Task.CompletedTask;
    }

    private string ReplaceToken(string pattern)
    {
        var now = DateTime.UtcNow;
        return pattern
            .Replace("{YYYY}", now.ToString("yyyy"))
            .Replace("{MM}", now.ToString("MM"))
            .Replace("{DD}", now.ToString("dd"))
            .Replace("{Quarter}", $"Q{(now.Month - 1) / 3 + 1}");
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
