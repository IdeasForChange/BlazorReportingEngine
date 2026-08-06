namespace Smbc.Risk.Core.Domain.Shared.Exceptions;

public class EntityTypeFieldIsNullValidationError(string? propertyName)
    : EntityValidationError($"{propertyName} cannot be Empty / Null!");
