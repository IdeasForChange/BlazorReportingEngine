using System.ComponentModel;

namespace Smbc.Risk.ReportingEngine.Domain.Shared.Enums;

public enum DatabaseAuthenticationType
{
    [Description("Integrated Authentication")]
    Integrated,
    [Description("Individual Authentication")]
    Individual
}
