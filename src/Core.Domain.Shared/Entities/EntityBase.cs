namespace Smbc.Risk.Core.Domain.Shared.Entities;

public abstract class EntityBase
{
    public long Id { get; set; }
    public int EntityVersion { get; set; } = 1;
    public DateTime EntityWrittenAt { get; set; } = DateTime.UtcNow;

    // Audting Fields 
    public bool IsActive { get; set; } = true;
    public string? CreatedBy { get; set; } = "System";
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public string? UpdatedBy { get; set; } = "System";
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    /// <inheritdoc />
    public override string ToString() => $"{GetType().Name} {Id}";
}
