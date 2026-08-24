using Smbc.Risk.ReportingEngine.Domain.Shared.DataTransferObjects;

namespace Smbc.Risk.ReportingEngine.Domain.Services;

public interface IDatabaseManagementService
{
    Task<IEnumerable<DatabaseConnectionDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<DatabaseConnectionDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<DatabaseConnectionDto> CreateAsync(DatabaseConnectionDto connectionDto, CancellationToken cancellationToken = default);
    Task<DatabaseConnectionDto> UpdateAsync(DatabaseConnectionDto connectionDto, CancellationToken cancellationToken = default);
    Task DeleteAsync(long id, CancellationToken cancellationToken = default);
}