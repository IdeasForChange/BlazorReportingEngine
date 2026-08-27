using Smbc.Risk.Core.Domain.Shared.Repositories;
using Smbc.Risk.ReportingEngine.Domain.Entities;
using Smbc.Risk.ReportingEngine.Domain.Shared.Enums;

namespace Smbc.Risk.ReportingEngine.Domain.Repositories;

public interface IReportRunnerQueueRepository : IBaseRepository<ReportRunnerQueue>
{
    Task<ReportRunnerQueue?> GetNextPendingItemAsync();
    Task<IEnumerable<ReportRunnerQueue>> GetQueueByFilterAsync(string filter);

    Task UpdateQueueStatusAsync(long queueItemId, QueueStatus status, string? errorMessage = null);
    Task CompleteQueueItemAsync(long queueItemId, string outputFilePath);
}
