using Microsoft.EntityFrameworkCore;
using Smbc.Risk.Core.Domain.Shared.Repositories;

namespace Smbc.Risk.ReportingEngine.Infrastructure.Data.EntityFramework.Repositories;

public abstract class BaseRepository<T>(ApplicationDbContext dbContext) : IBaseRepository<T> 
    where T : class
{
    protected readonly ApplicationDbContext _dbContext = dbContext;

    // CREATE
    public async Task<T> Create(T entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.Set<T>().AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync();
        return entity;
    }

    // READ & READ ALL
    public async Task<IEnumerable<T>> GetAll(CancellationToken cancellationToken = default)
    {
        var resut = await _dbContext.Set<T>().AsNoTracking().ToListAsync(cancellationToken);
        return resut;
    }

    public async Task<T?> GetById<TId>(TId id, CancellationToken cancellationToken = default)
    {
        var resut = await _dbContext.Set<T>().FindAsync([id], cancellationToken: cancellationToken);
        return resut;
    }

    // UPDATE 
    public async Task<T> Update(T entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Set<T>().Update(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    // DELETE 
    public async Task<bool> Delete(T entity, CancellationToken cancellationToken = default)
    {
        try
        {
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
    public async Task<int> Count(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<T>().CountAsync(cancellationToken);
    }

    public abstract Task<bool> Exists(long id, CancellationToken cancellationToken = default);
}
