using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smbc.ReportingEngine.Domain.Entities;

namespace Smbc.ReportingEngine.Infrastructure.Data.EntityFramework.Configurations;

public class SystemParameterTypeConfiguration : IEntityTypeConfiguration<SystemParameterType>
{
    public void Configure(EntityTypeBuilder<SystemParameterType> builder)
    {
        builder.ToTable("SystemParameterType");

        // Common Properties
        builder.HasKey(j => j.Id);

        // Main Payload
        builder.Property(j => j.Code).IsUnicode().HasMaxLength(100).IsRequired();
        builder.Property(j => j.Name).HasMaxLength(200).IsRequired();
        builder.Property(j => j.Description).HasMaxLength(1000);

        builder.Property(j => j.EntityVersion);
        builder.Property(j => j.EntityWrittenAt);

        // Audit Flags
        builder.Property(j => j.IsActive);
        builder.Property(j => j.CreatedBy);
        builder.Property(j => j.CreatedAtUtc);
        builder.Property(j => j.UpdatedBy);
        builder.Property(j => j.UpdatedAtUtc);
    }
}
