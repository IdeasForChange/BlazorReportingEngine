using ClosedXML.Excel;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Smbc.Risk.ReportingEngine.Domain.Entities;
using Smbc.Risk.ReportingEngine.Domain.Repositories;
using Smbc.Risk.ReportingEngine.Domain.Shared.Enums;

namespace Smbc.Risk.ReportingEngine.Infrastructure.BackgroundWorkers;

public class ReportRunnerWorker(ILogger<ReportRunnerWorker> logger, IReportRunnerRepository repository) : BackgroundService
{
    private readonly ILogger<ReportRunnerWorker> _logger = logger;
    private readonly IReportRunnerRepository _repository = repository;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Multi-threaded parallel processing scaled to hardware threads
        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = Math.Max(1, Environment.ProcessorCount / 2),
            CancellationToken = stoppingToken
        };

        while (!stoppingToken.IsCancellationRequested)
        {
            var pendingJobs = await _repository.GetPendingQueueItemsAsync();

            await Parallel.ForEachAsync(pendingJobs, options, async (job, token) =>
            {
                await ProcessReportJobAsync(job, token);
            });

            await Task.Delay(3000, stoppingToken);
        }
    }

    private async Task ProcessReportJobAsync(ReportRunnerQueue job, CancellationToken cancellationToken)
    {
        try
        {
            job.Status = QueueStatus.Processing;
            job.ProgressPercentage = 10;
            await _repository.UpdateQueueItemAsync(job);

            var template = job.ReportTemplate!;
            var tempOutputFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.xlsx");
            File.Copy(template.FilePath, tempOutputFile, overwrite: true);

            using var workbook = new XLWorkbook(tempOutputFile);

            // Validate Excel ranges against configured Metrics
            var fileRanges = workbook.DefinedNames.Select(n => n.Name).ToHashSet();
            var activeMetrics = template.Metrics.Where(m => m.IsActive).ToList();

            foreach (var metric in activeMetrics)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (!fileRanges.Contains(metric.NamedRange))
                {
                    _logger.LogWarning("Metric named range {Range} not found in template file.", metric.NamedRange);
                    continue;
                }

                // Execute SQL Query & populate range (Simulated execution)
                var range = workbook.DefinedName(metric.NamedRange).Ranges.First();
                range.Cell(1, 1).Value = "Executed Result"; // Inject dynamic data here

                job.ProgressPercentage += (80 / Math.Max(1, activeMetrics.Count));
                await _repository.UpdateQueueItemAsync(job);
            }

            var finalFileName = $"{template.FileNamePattern}_{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx";
            var finalPath = Path.Combine(template.OutputDirectory, finalFileName);

            Directory.CreateDirectory(template.OutputDirectory);
            workbook?.SaveAs(finalPath);

            job.Status = QueueStatus.Completed;
            job.ProgressPercentage = 100;
            job.OutputFilePath = finalPath;
        }
        catch (OperationCanceledException)
        {
            job.Status = QueueStatus.Cancelled;
            job.ErrorMessage = "Execution killed by user.";
        }
        catch (Exception ex)
        {
            job.Status = QueueStatus.Failed;
            job.ErrorMessage = ex.Message;
        }
        finally
        {
            await _repository.UpdateQueueItemAsync(job);
        }
    }
}