namespace Smbc.Risk.Core.Domain.Shared.Exceptions;

public class EntityValidationError(string? message)
{
    public string? Message { get; } = message;
}
