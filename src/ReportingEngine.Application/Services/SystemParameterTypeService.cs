using AutoMapper;
using Smbc.Risk.ReportingEngine.Application.DataTransferObjects;
using Smbc.Risk.ReportingEngine.Domain.Repositories;
using Smbc.Risk.ReportingEngine.Domain.Services;
using System.Linq.Expressions;

namespace Smbc.Risk.ReportingEngine.Application.Services;

public class SystemParameterTypeService(ISystemParameterTypeRepository repository, IMapper mapper) : ISystemParameterTypeService
{
    private readonly ISystemParameterTypeRepository _repository = repository;
    private readonly IMapper _mapper = mapper;

    public Task<int> Count(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<SystemParameterTypeDto> Create(SystemParameterTypeDto entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Delete(SystemParameterTypeDto entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Exists(long id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<SystemParameterTypeDto>> Find(Expression<Func<SystemParameterTypeDto, bool>> predicate, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<SystemParameterTypeDto>> GetAll(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<SystemParameterTypeDto?> GetById(long id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<SystemParameterTypeDto> Update(SystemParameterTypeDto entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
