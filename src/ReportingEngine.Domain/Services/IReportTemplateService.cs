using Smbc.Risk.Core.Domain.Shared.Services;
using Smbc.Risk.ReportingEngine.Domain.Shared.DataTransferObjects;

namespace Smbc.Risk.ReportingEngine.Domain.Services;

public interface IReportTemplateService 
{
    Task<IEnumerable<ReportTemplateDto>> GetAllReportsAsync(CancellationToken cancellationToken = default);
    Task<ReportTemplateDto?> GetReportByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<ReportTemplateDto> CreateReportWithTemplateAsync(CreateReportTemplateRequest request, CancellationToken cancellationToken = default);
    Task UpdateMetricAsync(ReportMetricDto metricDto, CancellationToken cancellationToken = default);
    Task AddParameterAsync(ReportParameterDto parameterDto, CancellationToken cancellationToken = default);
    Task DeleteReportAsync(long id, CancellationToken cancellationToken = default);
}