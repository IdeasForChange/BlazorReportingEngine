using AutoMapper;
using Microsoft.Extensions.Logging;
using Smbc.Risk.Core.Application.Services;
using Smbc.Risk.ReportingEngine.Domain.Entities;
using Smbc.Risk.ReportingEngine.Domain.Repositories;
using Smbc.Risk.ReportingEngine.Domain.Services;
using Smbc.Risk.ReportingEngine.Domain.Shared.DataTransferObjects;
using Smbc.Risk.ReportingEngine.Domain.Shared.Enums;

namespace Smbc.Risk.ReportingEngine.Application.Services;

public class DatabaseManagementService(
    ILogger<DatabaseManagementService> logger,
    IMapper mapper,
    IDatabaseConnectionRepository repository
    ) : IDatabaseManagementService
{
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<DatabaseManagementService> _logger = logger;
    private readonly IDatabaseConnectionRepository _repository = repository;

    public async Task<IEnumerable<DatabaseConnectionDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        // Retrieve all database connection records from the database using the repository
        var results = await _repository.GetAllAsync(false, cancellationToken);

        // Map the results to DTOs and return them
        return _mapper.Map<IEnumerable<DatabaseConnectionDto>>(results);
    }

    public async Task<DatabaseConnectionDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        return _mapper.Map<DatabaseConnectionDto>(entity);
    }

    public async Task<DatabaseConnectionDto> CreateAsync(DatabaseConnectionDto dto, CancellationToken cancellationToken = default)
    {
        var entity = _mapper.Map<DatabaseConnection>(dto);
        await _repository.CreateAsync(entity, cancellationToken);
        return _mapper.Map<DatabaseConnectionDto>(entity);
    }

    public async Task<DatabaseConnectionDto> UpdateAsync(DatabaseConnectionDto dto, CancellationToken cancellationToken = default)
    {
        if(dto.Id == null)
        {
            throw new ArgumentException("Database Connection ID is required for update.");
        }

        var existing = await _repository.GetByIdAsync(dto.Id.Value, cancellationToken);
        if (existing != null)
        {
            _mapper.Map(dto, existing);

            await _repository.UpdateAsync(existing, cancellationToken);
            return _mapper.Map<DatabaseConnectionDto>(existing);
        }

        throw new KeyNotFoundException("Database Connection not found.");
    }

    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(id, cancellationToken);
    }
}
