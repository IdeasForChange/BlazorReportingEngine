using Smbc.Risk.ReportingEngine.Domain.Shared.DataTransferObjects;

namespace Smbc.Risk.ReportingEngine.Domain.Services;

public interface IReportManagementService
{
    Task<IEnumerable<ReportMasterDto>> GetAllReportsAsync(bool includeInactive = false, CancellationToken cancellationToken = default);
    Task<ReportMasterDto> CreateReportAsync(SaveReportMasterDto dto, CancellationToken cancellationToken = default);
    Task<ReportMasterDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<ReportMasterDto> UpdateAsync(ReportMasterDto dto, string user, CancellationToken cancellationToken = default);
    Task DeleteAsync(long id, bool hardDelete = false, CancellationToken cancellationToken = default);

}
