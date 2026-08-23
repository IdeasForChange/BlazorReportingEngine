using Smbc.Risk.ReportingEngine.Domain.Shared.DataTransferObjects;

namespace Smbc.Risk.ReportingEngine.Domain.Services;

public interface IReportManagementService
{
    Task<IEnumerable<ReportMasterDto>> GetAllReportsAsync(bool includeInactive = false, CancellationToken cancellationToken = default);
    Task<ReportMasterDto> CreateReportAsync(SaveReportMasterDto dto, CancellationToken cancellationToken = default);
}
