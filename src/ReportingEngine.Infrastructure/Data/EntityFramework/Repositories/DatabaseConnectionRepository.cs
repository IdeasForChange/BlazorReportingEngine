using Smbc.Risk.ReportingEngine.Domain.Entities;
using Smbc.Risk.ReportingEngine.Domain.Repositories;

namespace Smbc.Risk.ReportingEngine.Infrastructure.Data.EntityFramework.Repositories;

public class DatabaseConnectionRepository(ApplicationDbContext dbContext)
    : BaseRepository<DatabaseConnection>(dbContext), IDatabaseConnectionRepository
{
}
