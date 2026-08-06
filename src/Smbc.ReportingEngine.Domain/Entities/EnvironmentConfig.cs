namespace Smbc.ReportingEngine.Domain.Entities;

public class EnvironmentConfig
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty; // e.g., "DEV", "SIT", "UAT", "PROD"
    public string Description { get; set; } = string.Empty;
}