using System.Linq.Expressions;

namespace Smbc.Risk.Core.Domain.Shared.Repositories;

/// <summary>
/// Generic repository interface following Repository Pattern.
/// </summary>
public interface IBaseRepository<T> where T : class
{
    Task<T?> GetById<TId>(TId id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAll(CancellationToken cancellationToken = default);
    Task<T> Create(T entity, CancellationToken cancellationToken = default);
    Task<T> Update(T entity, CancellationToken cancellationToken = default);
    Task<bool> Delete(T entity, CancellationToken cancellationToken = default);
    Task<bool> Exists(long id, CancellationToken cancellationToken = default);
    Task<int> Count(CancellationToken cancellationToken = default);
}
