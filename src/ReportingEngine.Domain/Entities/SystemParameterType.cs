using Smbc.Risk.Core.Domain.Shared.Entities;

namespace Smbc.ReportingEngine.Domain.Entities;

public class SystemParameterType : EntityBase
{
    public required string Code { get; set; }   // e.g., "CONNECTION_STRING"
    public required string Name { get; set; }   // e.g., "Connection String"
    public string? Description { get; set; }
}
