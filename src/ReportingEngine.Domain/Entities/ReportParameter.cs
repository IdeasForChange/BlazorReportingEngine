using Smbc.Risk.Core.Domain.Shared.Entities;
using Smbc.Risk.ReportingEngine.Domain.Shared.Enums;

namespace Smbc.Risk.ReportingEngine.Domain.Entities;

public class ReportParameter : EntityBase
{
    public long ReportTemplateId { get; set; }
    public string Name { get; set; } = string.Empty;
    public ParameterType Type { get; set; } = ParameterType.Text;
    public bool IsRequired { get; set; }

    public ReportTemplate? ReportTemplate { get; set; }
}
