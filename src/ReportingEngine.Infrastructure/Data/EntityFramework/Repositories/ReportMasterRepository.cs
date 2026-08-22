using Smbc.Risk.ReportingEngine.Domain.Entities;
using Smbc.Risk.ReportingEngine.Domain.Repositories;

namespace Smbc.Risk.ReportingEngine.Infrastructure.Data.EntityFramework.Repositories;

public class ReportMasterRepository (ApplicationDbContext dbContext) 
    : BaseRepository<ReportMaster>(dbContext), IReportMasterRepository
{
}
