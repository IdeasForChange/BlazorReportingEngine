using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smbc.Risk.ReportingEngine.Domain.Shared.DataTransferObjects;

public record ReportTemplateDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int TemplateVersion { get; set; } = 1;
    public string TemplatePath { get; set; } = string.Empty;
    public string ReportNamePattern { get; set; } = string.Empty;
    public string ReportDirectory { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public List<ReportMetricDto> Metrics { get; set; } = [];
    public List<ReportParameterDto> Parameters { get; set; } = [];
    public string? TemplateName => TemplatePath != null ? new FileInfo(TemplatePath)?.Name : null;
}