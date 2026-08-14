using AutoMapper;
using Smbc.ReportingEngine.Domain.Entities;
using Smbc.Risk.ReportingEngine.Application.DataTransferObjects;
using Smbc.Risk.ReportingEngine.Domain.Repositories;
using Smbc.Risk.ReportingEngine.Domain.Services;
using System.Linq.Expressions;

namespace Smbc.Risk.ReportingEngine.Application.Services;

public class SystemParameterTypeService(ISystemParameterTypeRepository repository, IMapper mapper) : ISystemParameterTypeService
{
    private readonly ISystemParameterTypeRepository _repository = repository;
    private readonly IMapper _mapper = mapper;

    public async Task<IEnumerable<SystemParameterTypeDto>> GetAll(CancellationToken cancellationToken = default)
    {
        var entities = await _repository.GetAll(cancellationToken);
        return _mapper.Map<IEnumerable<SystemParameterTypeDto>>(entities);
    }

    public async Task<SystemParameterTypeDto?> GetById(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetById(cancellationToken);
        return _mapper.Map<SystemParameterTypeDto?>(entity);
    }

    public async Task<SystemParameterTypeDto> Create(SystemParameterTypeDto createDto, CancellationToken cancellationToken = default)
    {
        // Business Validation
        if (string.IsNullOrWhiteSpace(createDto.Code))
        {
            throw new ArgumentException("Code cannot be empty.", nameof(createDto.Code));
        }

        if (await _repository.ExistsByCode(createDto.Code, cancellationToken: cancellationToken))
        {
            throw new InvalidOperationException($"SystemParameterType with Code '{createDto.Code}' already exists.");
        }

        // Convert the DTO to Entity
        var entity = _mapper.Map<SystemParameterType>(createDto);
        entity.CreatedBy = entity.CreatedBy ?? "System";
        entity.CreatedAtUtc = DateTime.UtcNow;
        entity.EntityWrittenAt = DateTime.UtcNow;

        var createdEntity = await _repository.Create(entity, cancellationToken);
        return _mapper.Map<SystemParameterTypeDto>(createdEntity);
    }

    public async Task<SystemParameterTypeDto> Update(SystemParameterTypeDto entity, CancellationToken cancellationToken = default)
    {
        var existing = await _repository.GetById(entity.Id, cancellationToken)
                    ?? throw new KeyNotFoundException($"Entity with ID {entity.Id} was not found.");

        if (await _repository.ExistsByCode(entity.Code, excludeId: entity.Id, cancellationToken))
            throw new InvalidOperationException($"Another record with Code '{entity.Code}' already exists.");

        _mapper.Map(entity, existing);

        // Update the rest of the properties
        existing.UpdatedBy = existing.UpdatedBy ?? "System";
        existing.UpdatedAtUtc = DateTime.UtcNow;

        // Update the Data
        await _repository.Update(existing, cancellationToken);

        return _mapper.Map<SystemParameterTypeDto>(existing);
    }

    public async Task<bool> Delete(SystemParameterTypeDto entity, CancellationToken cancellationToken = default)
    {
        var existing = await _repository.GetById(entity.Id, cancellationToken)
                    ?? throw new KeyNotFoundException($"Entity with ID {entity.Id} was not found.");

        return await _repository.Delete(existing, cancellationToken);
    }

    public async Task<int> Count(CancellationToken cancellationToken = default)
    {
        return await _repository.Count(cancellationToken);
    }

    public async Task<bool> Exists(long id, CancellationToken cancellationToken = default)
    {
        return await _repository.Exists(id, cancellationToken);
    }

    public async Task<IEnumerable<SystemParameterTypeDto>> Find(Expression<Func<SystemParameterTypeDto, bool>> predicate, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
