using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Smbc.Risk.ReportingEngine.Domain.Entities;
using Smbc.Risk.ReportingEngine.Domain.Services;
using System.Collections.Concurrent;

namespace Smbc.Risk.ReportingEngine.Infrastructure.BackgroundServices;

public class ReportRunnerQueueWorker : BackgroundService
{
    private readonly ReportJobChannel _jobChannel;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ConcurrentDictionary<long, CancellationTokenSource> _runningJobs = new();
    private readonly SemaphoreSlim _semaphore;
    private readonly int _maxConcurrentJobs = 4;
    private readonly TimeSpan _pollingInterval = TimeSpan.FromSeconds(15);
    private readonly ILogger<ReportRunnerQueueWorker> _logger;

    public ReportRunnerQueueWorker(
    ReportJobChannel jobChannel,
    IServiceScopeFactory scopeFactory,
    ILogger<ReportRunnerQueueWorker> logger)
    {
        _logger = logger;
        _jobChannel = jobChannel;
        _scopeFactory = scopeFactory;
        _semaphore = new SemaphoreSlim(_maxConcurrentJobs);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // 1. Initial Startup Recovery: Drain pending jobs from Database
        await DrainPendingJobsFromDatabaseAsync(stoppingToken);

        // 2. Main Processing Loop (Hybrid Signal + Periodic Timer)
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Wait for either a new channel signal OR a timer tick (polling fallback)
                using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
                timeoutCts.CancelAfter(_pollingInterval);

                try
                {
                    // If an item is pushed via API, handle it immediately
                    if (await _jobChannel.Reader.WaitToReadAsync(timeoutCts.Token))
                    {
                        while (_jobChannel.Reader.TryRead(out long jobId))
                        {
                            _ = ProcessJobWithSemaphoreAsync(jobId, stoppingToken);
                        }
                    }
                }
                catch (OperationCanceledException) when (!stoppingToken.IsCancellationRequested)
                {
                    // Timeout hit: Polling fallback to check if DB has stuck or missed pending jobs
                    await DrainPendingJobsFromDatabaseAsync(stoppingToken);
                }
            }
            catch (Exception ex)
            {
                // Prevent background service crash on unexpected error
                await Task.Delay(2000, stoppingToken);
            }
        }
    }

    private async Task DrainPendingJobsFromDatabaseAsync(CancellationToken stoppingToken)
    {
        int availableSlots = _semaphore.CurrentCount;
        if (availableSlots <= 0) return;

        using var scope = _scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IReportRunnerQueueService>();

        // Claim pending jobs up to available thread slots
        var pendingJobIds = await service.ClaimPendingJobIdsAsync(availableSlots, stoppingToken);

        foreach (var jobId in pendingJobIds)
        {
            _ = ProcessJobWithSemaphoreAsync(jobId, stoppingToken);
        }
    }

    private async Task ProcessJobWithSemaphoreAsync(long jobId, CancellationToken stoppingToken)
    {
        // Skip if already being processed by another task on this node
        if (_runningJobs.ContainsKey(jobId)) return;

        await _semaphore.WaitAsync(stoppingToken);

        _ = Task.Run(async () =>
        {
            var jobCts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
            if (!_runningJobs.TryAdd(jobId, jobCts))
            {
                _semaphore.Release();
                return;
            }

            try
            {
                using var scope = _scopeFactory.CreateScope();
                var runnerService = scope.ServiceProvider.GetRequiredService<IReportRunnerQueueService>();

                // Execute job processing
                await runnerService.ProcessQueueItemAsync(jobId, jobCts.Token);
            }
            finally
            {
                _runningJobs.TryRemove(jobId, out _);
                jobCts.Dispose();
                _semaphore.Release();
            }
        }, stoppingToken);
    }

    public void CancelJobExecution(long jobId)
    {
        if (_runningJobs.TryGetValue(jobId, out var cts))
        {
            cts.Cancel();
        }
    }
}
