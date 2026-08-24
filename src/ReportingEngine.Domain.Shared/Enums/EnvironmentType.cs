using System.ComponentModel;

namespace Smbc.Risk.ReportingEngine.Domain.Shared.Enums;

public enum EnvironmentType
{
    [Description("Development")]
    Development,
    [Description("System Test")]
    SystemTest,
    [Description("User Acceptance Test")]
    UserAcceptanceTest,
    [Description("Pre-Production")]
    PreProduction,
    [Description("Production")] 
    Production
}