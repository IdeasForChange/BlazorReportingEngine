using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smbc.Risk.ReportingEngine.Domain.Entities;

namespace Smbc.Risk.ReportingEngine.Infrastructure.Data.EntityFramework.Configurations;

public class ReportParameterConfiguration : IEntityTypeConfiguration<ReportParameter>
{
    public void Configure(EntityTypeBuilder<ReportParameter> builder)
    {
        builder.ToTable("ReportParameter");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(255);
        builder.Property(e => e.Type).HasConversion<int>().IsRequired();

        builder.HasIndex(e => e.ReportTemplateId);
    }
}