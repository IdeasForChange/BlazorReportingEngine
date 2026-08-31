namespace Smbc.Risk.ReportingEngine.Domain.Services;

public interface IReportRunnerQueueService
{
    Task<List<long>> ClaimPendingJobIdsAsync(int batchSize, CancellationToken cancellationToken);
    Task<long> EnqueueJobAsync(long reportMasterId, string parameterValuesJson, string requestedBy, CancellationToken cancellationToken);
    Task ProcessQueueItemAsync(long jobId, CancellationToken cancellationToken);
    Task CancelJobAsync(long jobId, CancellationToken cancellationToken);
}