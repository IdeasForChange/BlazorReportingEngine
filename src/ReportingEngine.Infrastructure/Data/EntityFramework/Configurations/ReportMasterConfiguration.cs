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

        builder.Property(e => e.Name).IsRequired().HasMaxLength(255);
        builder.Property(e => e.Description).HasMaxLength(1000);
        builder.Property(e => e.ReportNamePattern).IsRequired().HasMaxLength(255);
        builder.Property(e => e.ReportDirectory).IsRequired().HasMaxLength(1000);

        // Common Table Items
        builder.Property(e => e.EntityVersion).HasDefaultValue(1).IsRequired();
        builder.Property(e => e.EntityWrittenAt).HasDefaultValueSql("GETUTCDATE()").IsRequired();

        // Audit Flags
        builder.Property(e => e.IsActive).HasDefaultValue(true).IsRequired();
        builder.Property(e => e.CreatedBy).HasMaxLength(256).HasDefaultValue("System");
        builder.Property(e => e.CreatedAtUtc).HasDefaultValueSql("GETUTCDATE()").IsRequired();
        builder.Property(e => e.UpdatedBy).HasMaxLength(256).HasDefaultValue("System");
        builder.Property(e => e.UpdatedAtUtc).HasDefaultValueSql("GETUTCDATE()").IsRequired();

        // Relationships
        builder.HasMany(e => e.Parameters)
            .WithOne(p => p.Report)
            .HasForeignKey(p => p.ReportId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_ReportParameter_ReportMaster");

        builder.HasMany(e => e.Templates)
            .WithOne(t => t.Report)
            .HasForeignKey(t => t.ReportId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_ReportTemplate_ReportMaster");
    }
}