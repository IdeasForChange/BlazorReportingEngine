using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smbc.Risk.ReportingEngine.Domain.Entities;

namespace Smbc.Risk.ReportingEngine.Infrastructure.Data.EntityFramework.Configurations;

public class ReportRunnerQueueConfiguration : IEntityTypeConfiguration<ReportRunnerQueue>
{
    public void Configure(EntityTypeBuilder<ReportRunnerQueue> builder)
    {
        builder.ToTable("ReportRunnerQueue", "Reporting");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Status).HasConversion<int>().IsRequired();
        builder.Property(e => e.OutputFilePath).HasMaxLength(1000);

        builder.HasIndex(e => e.ReportMasterId);
        builder.HasIndex(e => e.Status);

        // Common Table Items
        builder.Property(e => e.EntityVersion).HasDefaultValue(1).IsRequired();
        builder.Property(e => e.EntityWrittenAt).HasDefaultValueSql("GETUTCDATE()").IsRequired();

        // Audit Flags
        builder.Property(e => e.IsActive).HasDefaultValue(true).IsRequired();
        builder.Property(e => e.CreatedBy).HasMaxLength(256).HasDefaultValue("System");
        builder.Property(e => e.CreatedAtUtc).HasDefaultValueSql("GETUTCDATE()").IsRequired();
        builder.Property(e => e.UpdatedBy).HasMaxLength(256).HasDefaultValue("System");
        builder.Property(e => e.UpdatedAtUtc).HasDefaultValueSql("GETUTCDATE()").IsRequired();

        // Relationship configuration
        builder.HasOne(e => e.ReportMaster)
               .WithMany(m => m.ReportRunnerQueues)
               .HasForeignKey(e => e.ReportMasterId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}