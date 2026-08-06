using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smbc.ReportingEngine.Domain.Entities;

namespace Smbc.ReportingEngine.Infrastructure.Data.EntityFramework.Configurations;

public class WorkspaceConfiguration : IEntityTypeConfiguration<Workspace>
{
    public void Configure(EntityTypeBuilder<Workspace> builder)
    {
        builder.ToTable("EnvironmentConfig");

        // Common Properties
        builder.HasKey(j => j.Id);
        builder.Property(j => j.EntityVersion);
        builder.Property(j => j.EntityWrittenAt);

        // Additional Payload
        builder.Property(j => j.Name).IsRequired().HasMaxLength(200);
        builder.Property(j => j.Description).HasMaxLength(1000);
    }
}
