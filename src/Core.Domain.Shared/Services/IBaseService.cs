using System.Linq.Expressions;

namespace Smbc.Risk.Core.Domain.Shared.Services;

/// <summary>
/// Generic repository interface following Repository Pattern.
/// </summary>
public interface IBaseService<TEntityDto> where TEntityDto : class
{
    Task<TEntityDto?> GetById(long id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntityDto>> GetAll(CancellationToken cancellationToken = default);
    Task<TEntityDto> Create(TEntityDto entity, CancellationToken cancellationToken = default);
    Task<TEntityDto> Update(TEntityDto entity, CancellationToken cancellationToken = default);
    Task<bool> Delete(TEntityDto entity, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntityDto>> Find(Expression<Func<TEntityDto, bool>> predicate, CancellationToken cancellationToken = default);
    Task<bool> Exists(long id, CancellationToken cancellationToken = default);
    Task<int> Count(CancellationToken cancellationToken = default);
}
