using ClosedXML.Excel;
using Smbc.Risk.ReportingEngine.Domain.Entities;
using Smbc.Risk.ReportingEngine.Domain.Repositories;
using Smbc.Risk.ReportingEngine.Domain.Services;
using Smbc.Risk.ReportingEngine.Domain.Shared.Enums;

namespace Smbc.Risk.ReportingEngine.Application.Services;

public class ReportService(IReportRepository repo) : IReportService
{
    public async Task<ReportTemplate> UploadTemplateAsync(string name, string description, Stream fileStream, string fileName)
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}_{fileName}");
        await using (var fs = File.Create(tempPath))
            await fileStream.CopyToAsync(fs);

        // Extract Named Ranges via CloseXML
        using var workbook = new XLWorkbook(tempPath);
        var namedRanges = workbook.DefinedNames
            .Select(dn => dn.Name)
            .Distinct()
            .Select(name => new ReportMetric { NamedRange = name, SqlQuery = "-- TODO: Configure Query" })
            .ToList();

        var template = new ReportTemplate
        {
            Name = name,
            Description = description,
            FilePath = tempPath,
            Metrics = namedRanges
        };

        return await repo.AddTemplateAsync(template);
    }

    public async Task EnqueueReportAsync(long templateId, Dictionary<string, string> parameterValues)
    {
        var entry = new ReportRunnerQueue
        {
            ReportTemplateId = templateId,
            ParameterPayloadJson = System.Text.Json.JsonSerializer.Serialize(parameterValues),
            Status = QueueStatus.Pending
        };
        await repo.AddQueueEntryAsync(entry);
    }
}
