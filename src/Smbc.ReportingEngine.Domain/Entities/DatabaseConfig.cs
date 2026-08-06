using Smbc.ReportingEngine.Domain.Shared.Enums;

namespace Smbc.ReportingEngine.Domain.Entities;

public class DatabaseConfig : BaseEntity
{
    public string ConnectionName { get; set; } = string.Empty;
    public DatabaseProvider Provider { get; set; }
    public string ConnectionString { get; set; } = string.Empty;

    public int EnvironmentId { get; set; }
    public EnvironmentConfig? EnvironmentConfig { get; set; }

    public bool? IsValid { get; set; }
    public string? LastValidationError { get; set; }
    public DateTime? LastValidatedAt { get; set; }
}
