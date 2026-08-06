using System.Linq.Expressions;

namespace Smbc.Risk.Core.Domain.Shared.Services;

/// <summary>
/// Generic repository interface following Repository Pattern.
/// </summary>
public interface IBaseService<TEntityDto> where TEntityDto : class
{
    Task<TEntityDto?> GetById(long id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntityDto>> GetAll(CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntityDto>> Find(Expression<Func<TEntityDto, bool>> predicate, CancellationToken cancellationToken = default);
    Task<TEntityDto> Add(TEntityDto entity, CancellationToken cancellationToken = default);
    Task<TEntityDto> Update(TEntityDto entity, CancellationToken cancellationToken = default);
    Task<bool> Delete(long id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(long id, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<TEntityDto, bool>>? predicate = null, CancellationToken cancellationToken = default);
}
