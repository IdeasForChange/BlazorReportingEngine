using Microsoft.EntityFrameworkCore;
using Smbc.Risk.Core.Domain.Shared.Entities;
using Smbc.Risk.Core.Domain.Shared.Repositories;
using System.Linq.Expressions;

namespace Smbc.Risk.ReportingEngine.Infrastructure.Data.EntityFramework.Repositories;

public abstract class BaseRepository<T>(ApplicationDbContext dbContext) : IBaseRepository<T>
    where T : EntityBase
{
    protected readonly ApplicationDbContext _dbContext = dbContext;

    // READ & READ ALL
    public virtual async Task<IEnumerable<T>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Set<T>().AsNoTracking();
        if (!includeInactive)
        {
            query = query.Where(e => e.IsActive);
        }
        var results = await query.ToListAsync(cancellationToken);
        return results;
    }

    public virtual async Task<T?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var result = await _dbContext.Set<T>().FindAsync([id], cancellationToken: cancellationToken);
        return result;
    }

    // CREATE
    public virtual async Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.Set<T>().AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync();
        return entity;
    }

    // UPDATE 
    public virtual async Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Set<T>().Update(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    // DELETE 
    public virtual async Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = await GetByIdAsync(id, cancellationToken);
            if (entity is null) return false;

            _dbContext.Set<T>().Remove(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    // COUNT 
    public virtual async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<T>().CountAsync(cancellationToken);
    }

    // EXISTS
    public virtual async Task<bool> ExistsAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<T>().AnyAsync(e => e.Id == id, cancellationToken);
    }

    // FIND ALL WITH PREDICATE
    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbContext.Set<T>().AsNoTracking().Where(predicate).ToListAsync();
    }

}
