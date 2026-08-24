using MudBlazor;

namespace Smbc.Risk.ReportingEngine.WebUI.Features.Themes.Store;

public static class AppTheme
{
    public static MudTheme CustomTheme => new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#004b35",
            Secondary = "#198754",
            Tertiary = "#20c997",
            Info = "#20c997",
            AppbarBackground = "#004b35",
            AppbarText = "#FFFFFF",
            Background = "#F8F9FA",
            DrawerBackground = "#FFFFFF",
            TextPrimary = "#212529"
        },
        PaletteDark = new PaletteDark
        {
            Primary = "#20c997",
            Secondary = "#198754",
            Tertiary = "#004b35",
            Info = "#20c997",
            AppbarBackground = "#003424",
            AppbarText = "#E9ECEF",
            Background = "#121212",
            DrawerBackground = "#1A1A1A",
            TextPrimary = "#E9ECEF"
        }
    };
}