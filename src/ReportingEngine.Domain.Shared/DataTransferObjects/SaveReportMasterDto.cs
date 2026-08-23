namespace Smbc.Risk.ReportingEngine.Domain.Shared.DataTransferObjects;

public class SaveReportMasterDto
{
    public long? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ReportNamePattern { get; set; } = string.Empty;
    public string ReportDirectory { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string FileName { get; set; } = string.Empty;
    public byte[] FileBytes { get; set; } = Array.Empty<byte>();
}
