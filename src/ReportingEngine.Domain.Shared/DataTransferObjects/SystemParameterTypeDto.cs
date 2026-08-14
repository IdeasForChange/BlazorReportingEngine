namespace Smbc.Risk.ReportingEngine.Application.DataTransferObjects;

public record SystemParameterTypeDto(
    long Id,
    string Code,
    string Name,
    string? Description,
    bool IsActive,
    string? CreatedBy,
    DateTime? CreatedAtUtc,
    string? UpdatedBy,
    DateTime? UpdatedAtUtc
);

public record CreateSystemParameterTypeDto(string Code, string Name, string? Description, string? CreatedBy);
public record UpdateSystemParameterTypeDto(long Id, string Code, string Name, string? Description, bool IsActive, string? UpdatedBy);
