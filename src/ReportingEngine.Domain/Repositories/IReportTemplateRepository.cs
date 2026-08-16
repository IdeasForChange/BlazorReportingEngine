using Smbc.Risk.ReportingEngine.Domain.Shared.DataTransferObjects;

namespace Smbc.Risk.ReportingEngine.Domain.Repositories;

public interface IReportTemplateRepository
{
    Task<long> UploadTemplateAsync(
        string name,
        string? description,
        string outputDirectory,
        string fileNamePattern,
        Stream fileStream,
        string fileName,
        List<ReportParameterDto> parameters,
        CancellationToken cancellationToken = default);
}
