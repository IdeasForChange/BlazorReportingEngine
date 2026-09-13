using Smbc.Risk.ReportingEngine.Domain.Shared.Enums;

namespace Smbc.Risk.ReportingEngine.Domain.Shared.DataTransferObjects;

public class ReportTemplateDto
{
    public long? Id { get; set; }
    public long ReportMasterId { get; set; }
    public string OriginalFileName { get; set; } = string.Empty;
    public string UploadedFileName { get; set; } = string.Empty;
    public string TemplatePath { get; set; } = string.Empty;
    public int TemplateVersion { get; set; } = 1;
    public SpreadsheetQueryType QueryType { get; set; } = SpreadsheetQueryType.QueryInCell;
    public long? DatabaseConnectionId { get; set; }
    public string? DefinedNameFilters { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public string FileName { get; set; } = string.Empty;
    public byte[] FileBytes { get; set; } = [];

    public List<ReportMetricDto> ReportMetrics { get; set; } = [];
}