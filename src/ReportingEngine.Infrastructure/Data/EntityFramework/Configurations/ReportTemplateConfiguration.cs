using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smbc.Risk.ReportingEngine.Domain.Entities;

namespace Smbc.Risk.ReportingEngine.Infrastructure.Data.EntityFramework.Configurations;

public class ReportTemplateConfiguration : IEntityTypeConfiguration<ReportTemplate>
{
    public void Configure(EntityTypeBuilder<ReportTemplate> builder)
    {
        // Set Schema and Table Name
        builder.ToTable("ReportTemplate");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name).IsRequired().HasMaxLength(255);
        builder.Property(e => e.FilePath).IsRequired().HasMaxLength(1000);
        builder.Property(e => e.OutputDirectory).IsRequired().HasMaxLength(1000);
        builder.Property(e => e.FileNamePattern).IsRequired().HasMaxLength(255);
        builder.Property(e => e.CreatedBy).HasMaxLength(256);
        builder.Property(e => e.UpdatedBy).HasMaxLength(256);

        builder.HasMany(e => e.Metrics)
            .WithOne(e => e.ReportTemplate)
            .HasForeignKey(e => e.ReportTemplateId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Parameters)
            .WithOne(e => e.ReportTemplate)
            .HasForeignKey(e => e.ReportTemplateId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.QueueItems)
            .WithOne(e => e.ReportTemplate)
            .HasForeignKey(e => e.ReportTemplateId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}