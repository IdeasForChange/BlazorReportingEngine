namespace Smbc.Risk.Core.Domain.Shared.Entities;

public abstract class EntityBase
{
    public long Id { get; set; }
    public int EntityVersion { get; set; } = 1;
    public DateTime EntityWrittenAt { get; set; }

    /// <inheritdoc />
    public override string ToString() => $"{GetType().Name} {Id}";
}
