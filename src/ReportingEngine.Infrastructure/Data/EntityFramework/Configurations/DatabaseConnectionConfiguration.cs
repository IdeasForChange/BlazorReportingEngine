using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smbc.Risk.ReportingEngine.Domain.Entities;

namespace Smbc.Risk.ReportingEngine.Infrastructure.Data.EntityFramework.Configurations;

public class DatabaseConnectionConfiguration : IEntityTypeConfiguration<DatabaseConnection>
{
    public void Configure(EntityTypeBuilder<DatabaseConnection> builder)
    {
        // Table Mapping
        builder.ToTable("DatabaseConnection", "Reporting");

        // Primary Key
        builder.HasKey(x => x.Id);

        // Common Table Items
        builder.Property(e => e.EntityVersion).HasDefaultValue(1).IsRequired();
        builder.Property(e => e.EntityWrittenAt).HasDefaultValueSql("GETUTCDATE()").IsRequired();

        // Connection Details Properties
        builder.Property(x => x.ConnectionName).IsRequired().HasMaxLength(100);
        builder.Property(x => x.DatabaseType).IsRequired().HasConversion<int>();
        builder.Property(x => x.Environment).IsRequired().HasConversion<int>();
        builder.Property(x => x.ServerHost).IsRequired().HasMaxLength(255);
        builder.Property(x => x.Port).IsRequired().HasDefaultValue(1433);
        builder.Property(x => x.DatabaseName).IsRequired().HasMaxLength(128);

        // Authentication Properties
        builder.Property(x => x.AuthenticationMethod).IsRequired().HasConversion<int>();
        builder.Property(x => x.UserId).HasMaxLength(100).IsRequired(false);
        builder.Property(x => x.Password).HasMaxLength(255).IsRequired(false);

        // Operational Properties
        builder.Property(x => x.TimeoutSeconds).IsRequired().HasDefaultValue(30);

        builder.Property(e => e.IsActive).HasDefaultValue(true).IsRequired();
        builder.Property(e => e.CreatedBy).HasMaxLength(256).HasDefaultValue("System");
        builder.Property(e => e.CreatedAtUtc).HasDefaultValueSql("GETUTCDATE()").IsRequired();
        builder.Property(e => e.UpdatedBy).HasMaxLength(256).HasDefaultValue("System");
        builder.Property(e => e.UpdatedAtUtc).HasDefaultValueSql("GETUTCDATE()").IsRequired();

        // Unique Constraint: ConnectionName must be unique per Environment
        builder.HasIndex(x => new { x.ConnectionName, x.Environment }).IsUnique();
        builder.HasIndex(e => e.Environment);
    }
}