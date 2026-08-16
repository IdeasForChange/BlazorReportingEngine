using AutoMapper;
using Smbc.Risk.Core.Application.Services;
using Smbc.Risk.ReportingEngine.Domain.Entities;
using Smbc.Risk.ReportingEngine.Domain.Repositories;
using Smbc.Risk.ReportingEngine.Domain.Services;
using Smbc.Risk.ReportingEngine.Domain.Shared.DataTransferObjects;
using Smbc.Risk.ReportingEngine.Domain.Shared.Enums;

namespace Smbc.Risk.ReportingEngine.Application.Services;

public class ReportTemplateService(
    IReportTemplateRepository templateRepository,
    IReportParameterRepository parameterRepository,
    IReportMetricRepository metricRepository,
    IExcelParserService excelParserService,
    IMapper mapper) : IReportTemplateService
{
    private readonly IReportTemplateRepository _templateRepository = templateRepository;
    private readonly IReportParameterRepository _parameterRepository = parameterRepository;
    private readonly IReportMetricRepository _metricRepository = metricRepository;
    private readonly IExcelParserService _excelParserService = excelParserService;
    private readonly IMapper _mapper = mapper;

    public async Task<IEnumerable<ReportTemplateDto>> GetAllReportsAsync(CancellationToken cancellationToken = default)
    {
        var reports = await _templateRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<ReportTemplateDto>>(reports);
    }

    public async Task<ReportTemplateDto?> GetReportByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var report = await _templateRepository.GetByIdAsync(id, cancellationToken);
        return _mapper.Map<ReportTemplateDto>(report);
    }

    public async Task<ReportTemplateDto> CreateReportWithTemplateAsync(CreateReportTemplateRequest request, CancellationToken cancellationToken = default)
    {
        // 1. Save file locally or to storage path
        var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
        Directory.CreateDirectory(uploadDir);
        var filePath = Path.Combine(uploadDir, $"{Guid.NewGuid()}_{request.FileName}");
        await File.WriteAllBytesAsync(filePath, request.FileBytes);

        // 2. Parse Named Ranges from Excel
        using var stream = new MemoryStream(request.FileBytes);
        var namedRanges = _excelParserService.ExtractNamedRanges(stream);

        // 3. Construct Entity & Metrics
        var report = new ReportTemplate
        {
            Name = request.Name,
            Description = request.Description,
            OutputDirectory = request.OutputDirectory,
            FileNamePattern = request.FileNamePattern,
            FilePath = filePath,
            Metrics = namedRanges.Select(nr => new ReportMetric
            {
                NamedRange = nr,
                DatabaseType = DatabaseType.SqlServer
            }).ToList()
        };

        await _templateRepository.AddAsync(report, cancellationToken);
        return _mapper.Map<ReportTemplateDto>(report);
    }

    public async Task UpdateMetricAsync(ReportMetricDto metricDto, CancellationToken cancellationToken = default)
    {
        var metric = await _metricRepository.GetByIdAsync(metricDto.Id, cancellationToken);
        if (metric is null) return;

        metric.SqlQuery = metricDto.SqlQuery;
        metric.DatabaseType = metricDto.DatabaseType;
        metric.MaxRows = metricDto.MaxRows;

        await _metricRepository.UpdateAsync(metric, cancellationToken);
    }

    public async Task AddParameterAsync(ReportParameterDto parameterDto, CancellationToken cancellationToken = default)
    {
        var parameter = _mapper.Map<ReportParameter>(parameterDto);
        await _parameterRepository.AddAsync(parameter, cancellationToken);
    }

    public async Task DeleteReportAsync(long id, CancellationToken cancellationToken = default)
    {
        await _templateRepository.DeleteAsync(id, cancellationToken);
    }
}
