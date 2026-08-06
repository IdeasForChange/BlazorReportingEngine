namespace Smbc.ReportingEngine.Domain.Entities;

public class EnvironmentConfig : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
