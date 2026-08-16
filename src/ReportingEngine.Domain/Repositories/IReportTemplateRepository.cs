using Smbc.Risk.Core.Domain.Shared.Repositories;
using Smbc.Risk.ReportingEngine.Domain.Entities;
using Smbc.Risk.ReportingEngine.Domain.Shared.DataTransferObjects;

namespace Smbc.Risk.ReportingEngine.Domain.Repositories;

public interface IReportTemplateRepository : IBaseRepository<ReportTemplate>
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
