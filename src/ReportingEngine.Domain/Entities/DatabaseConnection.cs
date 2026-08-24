using Smbc.Risk.Core.Domain.Shared.Entities;
using Smbc.Risk.ReportingEngine.Domain.Shared.Enums;

namespace Smbc.Risk.ReportingEngine.Domain.Entities;

public class DatabaseConnection : EntityBase
{
    public string ConnectionName { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public string ServerHost { get; set; } = string.Empty;
    public int Port { get; set; } = 1433;
    public DatabaseType DatabaseType { get; set; } = DatabaseType.SqlServer;
    public EnvironmentType Environment { get; set; } = EnvironmentType.Development;
    public DatabaseAuthenticationType AuthenticationMethod { get; set; } = DatabaseAuthenticationType.Integrated;
    public string UserId { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int TimeoutSeconds { get; set; } = 30;

    public ICollection<ReportMetric> ReportMetrics { get; set; } = [];
    public bool IsReadOnly => Environment == EnvironmentType.Production;
}
