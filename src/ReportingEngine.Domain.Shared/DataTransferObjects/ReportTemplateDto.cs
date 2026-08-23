using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smbc.Risk.ReportingEngine.Domain.Shared.DataTransferObjects;

public record ReportTemplateDto
{
    public long Id { get; set; }
    public long ReportId { get; set; }
    public string TemplateFileName { get; set; } = string.Empty;
    public string TemplatePath { get; set; } = string.Empty;
    public int TemplateVersion { get; set; } = 1;
    public bool IsActive { get; set; } = true;
    public List<ReportMetricDto> Metrics { get; set; } = [];
}