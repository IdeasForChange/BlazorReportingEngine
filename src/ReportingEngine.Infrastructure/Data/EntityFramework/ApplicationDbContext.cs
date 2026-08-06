using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Smbc.ReportingEngine.Domain.Entities;
using Smbc.ReportingEngine.Domain.Shared.Enums;
using Smbc.ReportingEngine.Infrastructure.Data.EntityFramework.Configurations;

namespace Smbc.Risk.ReportingEngine.Infrastructure.Data.EntityFramework;

/// <summary>
/// EF Core DbContext supporting SQL Server, PostgreSQL, and SQLite.
/// </summary>
public class ApplicationDbContext : DbContext
{
    private readonly DatabaseProvider _provider;
    private readonly string? _connectionString;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public ApplicationDbContext(DatabaseProvider provider, string connectionString)
    {
        _provider = provider;
        _connectionString = connectionString;
    }

    public DbSet<Workspace> Workspaces { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            switch (_provider)
            {
                case DatabaseProvider.SqlServer:
                    optionsBuilder.UseSqlServer(_connectionString);
                    break;
                //case DatabaseProvider.PostgreSql:
                //    optionsBuilder.UseNpgsql(_connectionString);
                //    break;
                case DatabaseProvider.Sqlite:
                    //optionsBuilder.UseSqlite(_connectionString);
                    break;
            }
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations
        modelBuilder.ApplyConfiguration(new WorkspaceConfiguration());
    }
}
