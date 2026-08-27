using System.Collections.Concurrent;

namespace Smbc.Risk.ReportingEngine.Domain.Shared.DataTransferObjects;

public class ReportExecutionContext
{
    public long QueueItemId { get; set; }
    public long ReportMasterId { get; set; }
    public string OutputDirectory { get; set; } = string.Empty;
    public string TemplatePath { get; set; } = string.Empty;
    public string GeneratedFilePath { get; set; } = string.Empty;

    // Concurrent Dictionary storing parameters + environment variables
    public ConcurrentDictionary<string, string> VariableContext { get; } = new(StringComparer.OrdinalIgnoreCase);
}