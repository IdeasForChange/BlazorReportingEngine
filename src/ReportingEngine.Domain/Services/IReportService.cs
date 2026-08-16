using Smbc.Risk.ReportingEngine.Domain.Entities;

namespace Smbc.Risk.ReportingEngine.Domain.Services;

public interface IReportService
{
    Task<ReportTemplate> UploadTemplateAsync(string name, string description, Stream fileStream, string fileName);
    Task EnqueueReportAsync(long templateId, Dictionary<string, string> parameterValues);
}
