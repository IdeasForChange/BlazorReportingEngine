namespace Smbc.Risk.ReportingEngine.Domain.Services;

public interface IReportProcessor
{
    Task ProcessQueueItemAsync(long queueId, CancellationToken cancellationToken);
}
