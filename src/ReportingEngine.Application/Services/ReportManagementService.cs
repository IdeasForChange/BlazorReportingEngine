using AutoMapper;
using Microsoft.Extensions.Logging;
using Smbc.Risk.Core.Application.Services;
using Smbc.Risk.ReportingEngine.Domain.Entities;
using Smbc.Risk.ReportingEngine.Domain.Repositories;
using Smbc.Risk.ReportingEngine.Domain.Services;
using Smbc.Risk.ReportingEngine.Domain.Shared.DataTransferObjects;
using Smbc.Risk.ReportingEngine.Domain.Shared.Enums;

namespace Smbc.Risk.ReportingEngine.Application.Services;

public class ReportManagementService(
    ILogger<ReportManagementService> logger,
    IMapper mapper,
    IExcelParserService excelParserService,
    IReportMasterRepository reportMasterRepository
    ) : IReportManagementService
{
    private readonly IMapper _mapper = mapper;
    private readonly IExcelParserService _excelParserService = excelParserService;
    private readonly ILogger<ReportManagementService> _logger = logger;
    private readonly IReportMasterRepository _reportMasterRepository = reportMasterRepository;

    public async Task<IEnumerable<ReportMasterDto>> GetAllReportsAsync(bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        // Retrieve all report master records from the database using the repository
        var results = await _reportMasterRepository.GetAllAsync(includeInactive, cancellationToken);

        // Map the results to DTOs and return them
        return _mapper.Map<IEnumerable<ReportMasterDto>>(results);
    }

    public async Task<ReportMasterDto> CreateReportAsync(SaveReportMasterDto dto, CancellationToken cancellationToken = default)
    {
        // 1. Save file locally or to storage path
        // TODO: Use configuration from the json file.
        var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
        Directory.CreateDirectory(uploadDir);
        var filePath = Path.Combine(uploadDir, $"{Guid.NewGuid()}_{dto.FileName}");
        await File.WriteAllBytesAsync(filePath, dto.FileBytes);

        // 2. Parse Named Ranges from Excel
        using var stream = new MemoryStream(dto.FileBytes);
        var namedRanges = _excelParserService.ExtractNamedRanges(stream);

        // 3. Construct Entity & Metrics
        var report = new ReportMaster
        {
            Name = dto.Name,
            Description = dto.Description,
            ReportDirectory = dto.ReportDirectory,
            ReportNamePattern = dto.ReportNamePattern,
            Templates =
            [
                new()
                {
                    TemplateFileName = dto.FileName,
                    TemplatePath = filePath,
                    TemplateVersion = 1,
                    Metrics = [.. namedRanges.Select(nr => new ReportMetric
                    {
                        NamedRange = nr,
                        DatabaseType = DatabaseType.SqlServer
                    })]
                }
            ]
        };

        await _reportMasterRepository.CreateAsync(report, cancellationToken);
        return _mapper.Map<ReportMasterDto>(report);
    }
}
    