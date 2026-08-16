using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smbc.Risk.ReportingEngine.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smbc.Risk.ReportingEngine.Infrastructure.Data.EntityFramework.Configurations;

public class ReportMetricConfiguration : IEntityTypeConfiguration<ReportMetric>
{
    public void Configure(EntityTypeBuilder<ReportMetric> builder)
    {
        builder.ToTable("ReportMetrics");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.NamedRange)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(m => m.SqlQuery)
            .IsRequired();

        builder.Property(m => m.DatabaseType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(m => m.MaxRows)
            .IsRequired(false);
    }
}