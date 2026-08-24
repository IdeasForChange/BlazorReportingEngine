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

        builder.Property(e => e.ReportMasterId).IsRequired();
        builder.Property(e => e.TemplateFileName).HasMaxLength(1000).IsRequired();
        builder.Property(e => e.TemplatePath).HasMaxLength(1000).IsRequired();
        builder.Property(e => e.TemplateVersion).HasDefaultValue(1).IsRequired();

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
        builder.HasIndex(e => e.ReportMasterId);

        // Relationships
        builder.HasMany(e => e.ReportMetrics)
            .WithOne(m => m.ReportTemplate)
            .HasForeignKey(m => m.ReportTemplateId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}