using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smbc.Risk.ReportingEngine.Domain.Entities;

namespace Smbc.Risk.ReportingEngine.Infrastructure.Data.EntityFramework.Configurations;

public class ReportMasterConfiguration : IEntityTypeConfiguration<ReportMaster>
{
    public void Configure(EntityTypeBuilder<ReportMaster> builder)
    {
        // Set Schema and Table Name
        builder.ToTable("ReportMaster");

        builder.HasKey(e => e.Id);
        // Common Table Items
        builder.Property(e => e.EntityVersion).HasDefaultValue(1).IsRequired();
        builder.Property(e => e.EntityWrittenAt).HasDefaultValueSql("GETUTCDATE()").IsRequired();

        builder.Property(e => e.Name).IsRequired().HasMaxLength(255);
        builder.Property(e => e.Description).HasMaxLength(1000);
        builder.Property(e => e.ReportNamePattern).IsRequired().HasMaxLength(255);
        builder.Property(e => e.ReportDirectory).IsRequired().HasMaxLength(1000);

        // Audit Flags
        builder.Property(e => e.IsActive).HasDefaultValue(true).IsRequired();
        builder.Property(e => e.CreatedBy).HasMaxLength(256).HasDefaultValue("System");
        builder.Property(e => e.CreatedAtUtc).HasDefaultValueSql("GETUTCDATE()").IsRequired();
        builder.Property(e => e.UpdatedBy).HasMaxLength(256).HasDefaultValue("System");
        builder.Property(e => e.UpdatedAtUtc).HasDefaultValueSql("GETUTCDATE()").IsRequired();

        // Relationships
        builder.HasMany(e => e.ReportParameters)
            .WithOne(p => p.ReportMaster)
            .HasForeignKey(p => p.ReportMasterId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.ReportTemplates)
            .WithOne(t => t.ReportMaster)
            .HasForeignKey(t => t.ReportMasterId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}