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
    public string FilePath { get; set; } = string.Empty;
    public int Version { get; set; } = 1;
    public string OutputDirectory { get; set; } = string.Empty;
    public string FileNamePattern { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public List<ReportMetricDto> Metrics { get; set; } = [];
    public List<ReportParameterDto> Parameters { get; set; } = [];
    public string? FileName => FilePath != null ? new FileInfo(FilePath)?.Name : null;
}