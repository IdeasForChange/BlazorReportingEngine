using Smbc.Risk.ReportingEngine.Domain.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smbc.Risk.ReportingEngine.Domain.Shared.DataTransferObjects;

public record ReportParameterDto
{
    public long Id { get; set; }
    public long ReportTemplateId { get; set; }
    public string Name { get; set; } = string.Empty;
    public ParameterType Type { get; set; } = ParameterType.Text;
    public bool IsRequired { get; set; }
}
