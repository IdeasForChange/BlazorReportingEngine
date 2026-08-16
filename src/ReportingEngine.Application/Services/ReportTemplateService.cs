using AutoMapper;
using Smbc.Risk.ReportingEngine.Domain.Entities;
using Smbc.Risk.ReportingEngine.Domain.Services;
using Smbc.Risk.ReportingEngine.Domain.Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Smbc.Risk.ReportingEngine.Application.Services;

public class ReportTemplateService(IReportTemplateService repository, IMapper mapper) : IReportTemplateService
{
    private readonly IReportTemplateService _repository = repository;
    private readonly IMapper _mapper = mapper;

    public async Task<IEnumerable<ReportTemplateDto>> GetAllAsync() =>
        _mapper.Map<IEnumerable<ReportTemplateDto>>(await _repository.GetAll());

    public async Task<ReportTemplateDto?> GetByIdAsync(long id) =>
        _mapper.Map<ReportTemplateDto>(await _repository.GetById(id));

    public async Task<ReportTemplateDto> CreateAsync(ReportTemplateDto dto)
    {
        var created = await _repository.Create(dto);
        return _mapper.Map<ReportTemplateDto>(created);
    }

    public async Task UpdateAsync(ReportTemplateDto dto)
    {
        await _repository.Update(dto);
    }

    //public async Task DeleteAsync(long id) => await _repository.Delete(id);

    public Task<ReportTemplateDto?> GetById(long id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<ReportTemplateDto>> GetAll(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ReportTemplateDto> Create(ReportTemplateDto entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ReportTemplateDto> Update(ReportTemplateDto entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Delete(ReportTemplateDto entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<ReportTemplateDto>> Find(Expression<Func<ReportTemplateDto, bool>> predicate, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Exists(long id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<int> Count(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
