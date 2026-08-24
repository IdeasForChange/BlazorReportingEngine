using Microsoft.EntityFrameworkCore;
using Smbc.Risk.ReportingEngine.Domain.Entities;
using Smbc.Risk.ReportingEngine.Domain.Repositories;

namespace Smbc.Risk.ReportingEngine.Infrastructure.Data.EntityFramework.Repositories;

public class ReportMasterRepository(ApplicationDbContext dbContext)
    : BaseRepository<ReportMaster>(dbContext), IReportMasterRepository
{
    public async Task DeleteOrInactivateAsync(long id, bool hardDelete = false, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity == null) return;

        if (hardDelete)
        {
            _dbContext.Set<ReportMaster>().Remove(entity);
        }
        else
        {
            entity.IsActive = false;
            entity.UpdatedAtUtc = DateTime.UtcNow;
            _dbContext.Set<ReportMaster>().Update(entity);
        }
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public override async Task<IEnumerable<ReportMaster>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        var resuls = await _dbContext.Set<ReportMaster>()
            .Include(x => x.ReportParameters)
            .Include(x => x.ReportTemplates)
                .ThenInclude(t => t.ReportMetrics)
            .Where(x => includeInactive || x.IsActive)
            .ToListAsync();

        return resuls;
    }

    public override async Task<ReportMaster?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var results = await _dbContext.Set<ReportMaster>()
            .Include(x => x.ReportParameters)
            .Include(x => x.ReportTemplates)
                .ThenInclude(t => t.ReportMetrics)
            .FirstOrDefaultAsync(x => x.Id == id);
        return results;
    }
}
