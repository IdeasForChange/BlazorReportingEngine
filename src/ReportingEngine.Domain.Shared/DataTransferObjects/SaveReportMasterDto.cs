using Smbc.Risk.ReportingEngine.Domain.Shared.Enums;

namespace Smbc.Risk.ReportingEngine.Domain.Shared.DataTransferObjects;

public class SaveReportMasterDto
{
    public long? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ReportNamePattern { get; set; } = string.Empty;
    public string ReportDirectory { get; set; } = string.Empty;
    public string UploadDirectory { get; set; } = string.Empty;
    public SpreadsheetQueryType QueryType { get; set; } = SpreadsheetQueryType.QueryInCell;
    public long? DatabaseConnectionId { get; set; }
    public string? DefinedNameFilters { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string FileName { get; set; } = string.Empty;
    public byte[] FileBytes { get; set; } = [];
}
