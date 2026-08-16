using Microsoft.EntityFrameworkCore;
using Smbc.ReportingEngine.Domain.Entities;
using Smbc.ReportingEngine.Infrastructure.Data.EntityFramework.Configurations;
using Smbc.Risk.Core.Domain.Shared.Entities;
using Smbc.Risk.ReportingEngine.Domain.Entities;

namespace Smbc.Risk.ReportingEngine.Infrastructure.Data.EntityFramework;

/// <summary>
/// EF Core DbContext supporting any database type.
/// </summary>
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<SystemParameterType> SystemParameterTypes { get; set; }
    public DbSet<ReportTemplate> ReportTemplates { get; set; }
    public DbSet<ReportMetric> ReportMetrics { get; set; }
    public DbSet<ReportParameter> ReportParameters { get; set; }
    public DbSet<ReportRunnerQueue> ReportRunnerQueues { get; set; }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<EntityBase>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAtUtc = DateTime.UtcNow;
                    entry.Entity.UpdatedAtUtc = DateTime.UtcNow;
                    entry.Entity.EntityWrittenAt = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAtUtc = DateTime.UtcNow;
                    entry.Entity.EntityWrittenAt = DateTime.UtcNow;
                    entry.Entity.EntityVersion++;
                    break;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Sets default schema for all tables mapped in this DbContext
        modelBuilder.HasDefaultSchema("Reporting");

        // Apply all configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
