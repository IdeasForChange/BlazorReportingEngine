using Smbc.ReportingEngine.Domain.Shared.Enums;

namespace Smbc.ReportingEngine.Domain.Shared.Services;

public interface IDatabaseConnectionValidatorService
{
    Task<(bool IsValid, string? ErrorMessage)> ValidateAsync(DatabaseProvider provider, string connectionString);
}
