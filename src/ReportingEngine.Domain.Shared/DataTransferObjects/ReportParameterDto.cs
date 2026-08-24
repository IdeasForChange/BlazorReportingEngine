using Smbc.Risk.ReportingEngine.Domain.Shared.Enums;

namespace Smbc.Risk.ReportingEngine.Domain.Shared.DataTransferObjects;

public class ReportParameterDto
{
    public long? Id { get; set; }
    public long ReportMasterId { get; set; }
    public string Name { get; set; } = string.Empty;
    public ParameterType ParameterType { get; set; } = ParameterType.Text;
    public bool IsRequired { get; set; } = false;
    public bool IsActive { get; set; } = true;
}
