using ClosedXML.Excel;
using Microsoft.Extensions.Logging;
using Smbc.Risk.Core.Application.Services;
using Smbc.Risk.ReportingEngine.Application.Interfaces;
using Smbc.Risk.ReportingEngine.Domain.Repositories;
using Smbc.Risk.ReportingEngine.Domain.Shared.DataTransferObjects;
using Smbc.Risk.ReportingEngine.Domain.Shared.Enums;
using System.Collections.Concurrent;
using System.Data;
using System.Text.Json;

namespace Smbc.Risk.ReportingEngine.Application.Services;

public class ReportRunnerWorkflow(
    IReportRunnerQueueRepository reportRunnerQueueRepository,
    IReportMasterRepository reportMasterRepository,
    IDynamicQueryExecutor dynamicQueryExecutor,
    IExcelParserService excelParserService,
    ILogger<ReportRunnerWorkflow> logger) : IReportRunnerWorkflow
{
    private readonly IReportRunnerQueueRepository _reportRunnerQueueRepository = reportRunnerQueueRepository;
    private readonly IReportMasterRepository _reportMasterRepository = reportMasterRepository;
    private readonly IDynamicQueryExecutor _dynamicQueryExecutor = dynamicQueryExecutor;
    private readonly IExcelParserService _excelParserService = excelParserService;
    private readonly ILogger<ReportRunnerWorkflow> _logger = logger;

    public async Task ExecuteAsync(long queueItemId, CancellationToken cancellationToken)
    {
        var context = new ReportExecutionContext { QueueItemId = queueItemId };

        // Step 1: Initialize Context with Environment Variables
        foreach (System.Collections.DictionaryEntry env in Environment.GetEnvironmentVariables())
        {
            if (env.Key != null && env.Value != null)
            {
                //context.VariableContext[$"ENV:{env.Key}"] = env.Value.ToString()!;
            }
        }

        // Step 2: Fetch Queue Item & Report Details
        var queueItem = await _reportRunnerQueueRepository.GetByIdAsync(queueItemId);
        if (queueItem == null || queueItem.Status == QueueStatus.Cancelled) return;

        await _reportRunnerQueueRepository.UpdateQueueStatusAsync(queueItemId, QueueStatus.Processing);

        var reportMaster = await _reportMasterRepository.GetByIdAsync(queueItem.ReportMasterId);
        if (reportMaster == null)
        {
            throw new InvalidOperationException($"ReportMaster with ID {queueItem.ReportMasterId} not found.");
        }

        var activeTemplate = reportMaster.ReportTemplates.OrderByDescending(t => t.TemplateVersion).FirstOrDefault();
        if (activeTemplate == null)
        {
            throw new InvalidOperationException($"No active ReportTemplate for Master ID {queueItem.ReportMasterId}.");
        }

        var metrics = activeTemplate.ReportMetrics;

        // Step 3: Deserialize Parameters & Merge into Execution Context
        if (!string.IsNullOrEmpty(queueItem.ParameterValuesJson))
        {
            var paramsDict = JsonSerializer.Deserialize<Dictionary<string, string>>(queueItem.ParameterValuesJson);
            if (paramsDict != null)
            {
                foreach (var kvp in paramsDict)
                {
                    context.VariableContext[kvp.Key] = kvp.Value;
                }
            }
        }

        // Step 4: Prepare Output File Path & File Copy
        string timeStamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
        string fileName = $"{reportMaster.ReportNamePattern}_{queueItemId}_{timeStamp}.xlsx";
        context.OutputDirectory = reportMaster.ReportDirectory;

        Directory.CreateDirectory(context.OutputDirectory);
        context.GeneratedFilePath = Path.Combine(context.OutputDirectory, fileName);

        File.Copy(activeTemplate.TemplatePath, context.GeneratedFilePath, overwrite: true);

        // Step 5, 6 & 7: ClosedXML Query Execution & Data Insertion
        using (var workbook = new XLWorkbook(context.GeneratedFilePath))
        {
            foreach (var metric in metrics)
            {
                cancellationToken.ThrowIfCancellationRequested();

                // Replace {{Parameter}} tags inside SQL metric query
                string processedSql = ReplaceQueryTags(metric.SqlQuery, context.VariableContext);

                // Execute SQL Query dynamically
                var dataTable = await _dynamicQueryExecutor.ExecuteQueryAsync(
                    metric.DatabaseConnectionId,
                    processedSql,
                    metric.MaxRows,
                    cancellationToken
                );

                if (dataTable.Rows.Count > 0)
                {
                    PopulateNamedRangeData(workbook, metric.NamedRange, dataTable);
                }
            }

            // Save changes to disk using ClosedXML engine
            workbook.Save();
        }

        // Complete queue status
        await _reportRunnerQueueRepository.CompleteQueueItemAsync(queueItemId, context.GeneratedFilePath);
    }

    private static void PopulateNamedRangeData(IXLWorkbook workbook, string namedRangeName, DataTable dataTable)
    {
        // Search Workbook Level or Worksheet Level Named Ranges
        if (!workbook.DefinedNames.TryGetValue(namedRangeName, out var namedRanges))
        {
            return;
        }

        foreach (var namedRange in namedRanges.Ranges)
        {
            if (namedRange == null) continue;

            var startCell = namedRange.FirstCell();

            // Insert DataTable starting from top-left anchor of defined Named Range
            // 'true' parameter prints column headers
            var insertedTable = startCell.InsertTable(dataTable, namedRangeName, true);

            // Apply formatting options
            insertedTable.Theme = XLTableTheme.TableStyleMedium2;
            insertedTable.ShowAutoFilter = true;
            startCell.Worksheet.Columns().AdjustToContents();
        }
    }

    private static string ReplaceQueryTags(string query, ConcurrentDictionary<string, string> context)
    {
        foreach (var kvp in context)
        {
            query = query.Replace($"{{{{{kvp.Key}}}}}", kvp.Value, StringComparison.OrdinalIgnoreCase);
        }
        return query;
    }
}
