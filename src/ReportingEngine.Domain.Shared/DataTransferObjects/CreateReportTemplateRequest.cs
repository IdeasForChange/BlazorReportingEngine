namespace Smbc.Risk.ReportingEngine.Domain.Shared.DataTransferObjects;

public class CreateReportTemplateRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string TemplatePath { get; set; } = string.Empty;
    public int TemplateVersion { get; set; } = 1;
    public string ReportNamePattern { get; set; } = string.Empty;
    public string ReportDirectory { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public byte[] FileBytes { get; set; } = Array.Empty<byte>();
}
