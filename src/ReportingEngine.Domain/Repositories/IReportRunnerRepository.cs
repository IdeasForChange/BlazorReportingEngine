using Smbc.Risk.ReportingEngine.Domain.Entities;

namespace Smbc.Risk.ReportingEngine.Domain.Repositories;

public interface IReportRunnerRepository
{
    Task<List<ReportRunnerQueue>> GetPendingQueueItemsAsync();
    Task UpdateQueueItemAsync(ReportRunnerQueue item);
}
