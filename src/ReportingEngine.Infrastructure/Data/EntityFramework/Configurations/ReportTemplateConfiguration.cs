using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smbc.Risk.ReportingEngine.Domain.Entities;

namespace Smbc.Risk.ReportingEngine.Infrastructure.Data.EntityFramework.Configurations;

public class ReportTemplateConfiguration : IEntityTypeConfiguration<ReportTemplate>
{
    public void Configure(EntityTypeBuilder<ReportTemplate> builder)
    {
        builder.ToTable("ReportTemplates");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Description)
            .HasMaxLength(1000);

        builder.Property(t => t.FilePath)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(t => t.OutputDirectory)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(t => t.FileNamePattern)
            .IsRequired()
            .HasMaxLength(200);

        // One-To-Many: ReportTemplate -> ReportMetrics
        builder.HasMany(t => t.Metrics)
            .WithOne(m => m.ReportTemplate)
            .HasForeignKey(m => m.ReportTemplateId)
            .OnDelete(DeleteBehavior.Cascade);

        // One-To-Many: ReportTemplate -> ReportParameters
        builder.HasMany(t => t.Parameters)
            .WithOne(p => p.ReportTemplate)
            .HasForeignKey(p => p.ReportTemplateId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
