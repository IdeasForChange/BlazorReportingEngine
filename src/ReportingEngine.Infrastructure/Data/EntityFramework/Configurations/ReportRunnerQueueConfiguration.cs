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

        // Relationship configuration
        builder.HasOne(e => e.ReportMaster)
               .WithMany(m => m.ReportRunnerQueues)
               .HasForeignKey(e => e.ReportMasterId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}