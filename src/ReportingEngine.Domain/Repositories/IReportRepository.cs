using Smbc.Risk.ReportingEngine.Domain.Entities;

namespace Smbc.Risk.ReportingEngine.Domain.Repositories;

public interface IReportRepository
{
    Task<ReportTemplate> AddTemplateAsync(ReportTemplate template);
    Task AddQueueEntryAsync(ReportRunnerQueue entry);
}
