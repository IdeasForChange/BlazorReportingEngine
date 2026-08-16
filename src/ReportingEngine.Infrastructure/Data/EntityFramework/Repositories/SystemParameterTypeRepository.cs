using Microsoft.EntityFrameworkCore;
using Smbc.ReportingEngine.Domain.Entities;
using Smbc.Risk.ReportingEngine.Domain.Repositories;

namespace Smbc.Risk.ReportingEngine.Infrastructure.Data.EntityFramework.Repositories;

public class SystemParameterTypeRepository(ApplicationDbContext dbContext) 
    : BaseRepository<SystemParameterType>(dbContext), ISystemParameterTypeRepository
{
    public override async Task<bool> Exists(long id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.SystemParameterTypes.AnyAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsByCode(string code, long? excludeId = null, CancellationToken cancellationToken = default)
    {
        return await _dbContext.SystemParameterTypes.AnyAsync(e => e.Code == code && (!excludeId.HasValue || e.Id != excludeId.Value), cancellationToken);
    }
}
