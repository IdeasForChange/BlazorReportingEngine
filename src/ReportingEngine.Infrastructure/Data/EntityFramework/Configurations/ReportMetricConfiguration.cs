using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smbc.Risk.ReportingEngine.Domain.Entities;

namespace Smbc.Risk.ReportingEngine.Infrastructure.Data.EntityFramework.Configurations;

public class ReportMetricConfiguration : IEntityTypeConfiguration<ReportMetric>
{
    public void Configure(EntityTypeBuilder<ReportMetric> builder)
    {
        builder.ToTable("ReportMetric");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.ReportTemplateId).IsRequired();
        builder.Property(e => e.DatabaseConnectionId).IsRequired(false);
        builder.Property(e => e.NamedRange).HasMaxLength(255).IsRequired();
        builder.Property(e => e.SqlQuery).HasColumnType("nvarchar(max)").IsRequired();
        builder.Property(e => e.MaxRows).IsRequired(false);

        // Common Table Items
        builder.Property(e => e.EntityVersion).HasDefaultValue(1).IsRequired();
        builder.Property(e => e.EntityWrittenAt).HasDefaultValueSql("GETUTCDATE()").IsRequired();

        // Audit Flags
        builder.Property(e => e.IsActive).HasDefaultValue(true).IsRequired();
        builder.Property(e => e.CreatedBy).HasMaxLength(256).HasDefaultValue("System");
        builder.Property(e => e.CreatedAtUtc).HasDefaultValueSql("GETUTCDATE()").IsRequired();
        builder.Property(e => e.UpdatedBy).HasMaxLength(256).HasDefaultValue("System");
        builder.Property(e => e.UpdatedAtUtc).HasDefaultValueSql("GETUTCDATE()").IsRequired();

        // Foreign Key Index
        builder.HasIndex(e => e.ReportTemplateId);
        builder.HasIndex(e => e.DatabaseConnectionId);
    }
}