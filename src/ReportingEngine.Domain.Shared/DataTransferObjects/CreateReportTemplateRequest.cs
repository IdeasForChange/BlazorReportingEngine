namespace Smbc.Risk.ReportingEngine.Domain.Shared.DataTransferObjects;

public class CreateReportTemplateRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string OutputDirectory { get; set; } = string.Empty;
    public string FileNamePattern { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public byte[] FileBytes { get; set; } = Array.Empty<byte>();
}
