using Smbc.ReportingEngine.Domain.Shared.Enums;

namespace Smbc.ReportingEngine.Domain.Shared.Repositories;

public interface IDatabaseConnectionValidator
{
    Task<(bool IsValid, string? ErrorMessage)> ValidateAsync(DatabaseProvider provider, string connectionString);
}
