using Smbc.Risk.Core.Domain.Shared.Entities;

namespace Smbc.ReportingEngine.Domain.Entities;

public class Workspace : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
