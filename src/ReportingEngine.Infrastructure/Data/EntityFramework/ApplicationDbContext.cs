using Microsoft.EntityFrameworkCore;
using Smbc.ReportingEngine.Domain.Entities;
using Smbc.ReportingEngine.Infrastructure.Data.EntityFramework.Configurations;

namespace Smbc.Risk.ReportingEngine.Infrastructure.Data.EntityFramework;

/// <summary>
/// EF Core DbContext supporting any database type.
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<SystemParameterType> SystemParameterTypes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations
        modelBuilder.ApplyConfiguration(new SystemParameterTypeConfiguration());
    }
}
