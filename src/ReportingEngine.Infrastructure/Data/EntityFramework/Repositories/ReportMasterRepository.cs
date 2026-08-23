using Microsoft.EntityFrameworkCore;
using Smbc.Risk.ReportingEngine.Domain.Entities;
using Smbc.Risk.ReportingEngine.Domain.Repositories;

namespace Smbc.Risk.ReportingEngine.Infrastructure.Data.EntityFramework.Repositories;

public class ReportMasterRepository (ApplicationDbContext dbContext) 
    : BaseRepository<ReportMaster>(dbContext), IReportMasterRepository
{
    public override async Task<IEnumerable<ReportMaster>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        var resuls = await _dbContext.Set<ReportMaster>()
            .Include(x => x.Parameters)
            .Include(x => x.Templates)
                .ThenInclude(t => t.Metrics)
            .Where(x => includeInactive || x.IsActive)
            .ToListAsync();

        return resuls;
    }

    public override async Task<ReportMaster?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var results = await _dbContext.Set<ReportMaster>()
            .Include(x => x.Parameters)
            .Include(x => x.Templates)
                .ThenInclude(t => t.Metrics)
            .FirstOrDefaultAsync(x => x.Id == id);
        return results;
    }
}
