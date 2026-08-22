using AutoMapper;
using Microsoft.Extensions.Logging;
using Smbc.Risk.ReportingEngine.Domain.Entities;
using Smbc.Risk.ReportingEngine.Domain.Repositories;
using Smbc.Risk.ReportingEngine.Domain.Services;
using Smbc.Risk.ReportingEngine.Domain.Shared.DataTransferObjects;

namespace Smbc.Risk.ReportingEngine.Application.Services;

public class ReportManagementService(
    ILogger<ReportManagementService> logger,
    IMapper mapper,
    IReportMasterRepository reportMasterRepository
    ) : IReportManagementService
{
    private readonly ILogger<ReportManagementService> _logger = logger;
    private readonly IMapper _mapper = mapper;
    private readonly IReportMasterRepository _reportMasterRepository = reportMasterRepository;

    public async Task<ReportMasterDto> CreateReportAsync(ReportMasterDto dto, CancellationToken cancellationToken = default)
    {
        var entity = _mapper.Map<ReportMaster>(dto);

        // Add the data to the database using the repository
        var result = await _reportMasterRepository.CreateAsync(entity, cancellationToken);

        // Map the result back to a DTO and return it
        return _mapper.Map<ReportMasterDto>(result);
    }

    public async Task<IEnumerable<ReportMasterDto>> GetAllReportsAsync(CancellationToken cancellationToken = default)
    {
        // Retrieve all report master records from the database using the repository
        var results = await _reportMasterRepository.GetAllAsync(cancellationToken);

        // Map the results to DTOs and return them
        return _mapper.Map<IEnumerable<ReportMasterDto>>(results);
    }
}
