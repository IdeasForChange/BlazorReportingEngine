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
        builder.Property(e => e.NamedRange).IsRequired().HasMaxLength(255);
        builder.Property(e => e.SqlQuery).IsRequired();
        builder.Property(e => e.DatabaseType).HasConversion<int>().IsRequired();

        builder.HasIndex(e => e.ReportTemplateId);
    }
}