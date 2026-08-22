using Smbc.Risk.ReportingEngine.Domain.Shared.DataTransferObjects;

namespace Smbc.Risk.ReportingEngine.Domain.Services;

public interface IReportManagementService
{
    Task<IEnumerable<ReportMasterDto>> GetAllReportsAsync(CancellationToken cancellationToken = default);
    Task<ReportMasterDto> CreateReportAsync(ReportMasterDto dto, CancellationToken cancellationToken = default);
}
