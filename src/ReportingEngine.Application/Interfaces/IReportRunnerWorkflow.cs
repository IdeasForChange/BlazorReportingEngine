namespace Smbc.Risk.ReportingEngine.Application.Interfaces;

public interface IReportRunnerWorkflow
{
    Task ExecuteAsync(long queueItemId, CancellationToken cancellationToken);
}
