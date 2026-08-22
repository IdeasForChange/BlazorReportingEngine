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
        builder.Property(j => j.EntityVersion);
        builder.Property(j => j.EntityWrittenAt);

        // Audit Flags
        builder.Property(j => j.IsActive);
        builder.Property(e => e.CreatedBy).HasMaxLength(256);
        builder.Property(j => j.CreatedAtUtc);
        builder.Property(e => e.UpdatedBy).HasMaxLength(256);
        builder.Property(j => j.UpdatedAtUtc);
    }
}