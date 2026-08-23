using Smbc.Risk.Core.Domain.Shared.Entities;
using Smbc.Risk.ReportingEngine.Domain.Shared.Enums;

namespace Smbc.Risk.ReportingEngine.Domain.Entities;

public class ReportParameter : EntityBase
{
    public long ReportId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ParameterType { get; set; } = 1;
    public bool IsRequired { get; set; } = false;

    public ReportMaster? Report { get; set; }
}
