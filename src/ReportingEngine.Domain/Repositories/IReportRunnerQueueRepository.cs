using Smbc.Risk.Core.Domain.Shared.Repositories;
using Smbc.Risk.ReportingEngine.Domain.Entities;
using Smbc.Risk.ReportingEngine.Domain.Shared.Enums;

namespace Smbc.Risk.ReportingEngine.Domain.Repositories;

public interface IReportRunnerQueueRepository : IBaseRepository<ReportRunnerQueue>
{
    Task<List<long>> ClaimPendingJobIdsAsync(int batchSize, CancellationToken cancellationToken);
    Task<ReportRunnerQueue?> GetNextPendingJobAsync(CancellationToken cancellationToken);
    Task UpdateJobStatusAsync(long jobId, QueueStatus status, int progress, string? outputFilePath = null, string? errorMessage = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<ReportRunnerQueue>> GetQueueByFilterAsync(string filter);
    Task CompleteQueueItemAsync(long queueItemId, string outputFilePath);

    //Task<ReportRunnerQueue?> GetJobByIdAsync(long jobId, CancellationToken cancellationToken);
    //Task<ReportMaster?> GetReportDetailsForExecutionAsync(long reportMasterId, CancellationToken cancellationToken);
    //Task<DatabaseConnection?> GetDatabaseConnectionAsync(long connectionId, CancellationToken cancellationToken);
}
