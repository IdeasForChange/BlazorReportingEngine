namespace Smbc.Risk.Core.Domain.Shared.Repositories;

/// <summary>
/// Generic repository interface following Repository Pattern.
/// </summary>
public interface IBaseRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default);
    Task<T?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default);
    Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(long id, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
}
