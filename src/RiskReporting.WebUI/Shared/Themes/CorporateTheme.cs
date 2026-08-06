using MudBlazor;

namespace Smbc.Risk.ReportingEngine.WebUI.Shared.Themes;

public class ThemeManager
{
    public static MudTheme CorporateTheme = new MudTheme()
    {
        PaletteLight = new PaletteLight()
        {
            Primary = "#005E44",                  // Trad Green
            PrimaryContrastText = "#FFFFFF",
            Secondary = "#B6CF48",                // Fresh Green
            SecondaryContrastText = "#1A2521",
            Tertiary = "#FF6F43",                 // Contrast Coral Accent
            TertiaryContrastText = "#FFFFFF",

            // Glassmorphic Overrides (Using semi-transparent RGBA strings)
            Background = "rgba(244, 246, 245, 0.7)", // Translucent off-white canvas
            Surface = "rgba(255, 255, 255, 0.45)",   // Frosted white glass surfaces

            TextPrimary = "#1A2521",
            TextSecondary = "#55635E",

            AppbarBackground = "#198754", // "rgba(25, 135, 84, 0.75)", // Deep Trad Green glass header
            AppbarText = "#FFFFFF",
            DrawerBackground = "rgba(255, 255, 255, 0.5)", // Sidebar glass
            DrawerText = "#1A2521",
            ActionDefault = "#55635E"
        },
        PaletteDark = new PaletteDark()
        {
            Primary = "#107A5E",
            PrimaryContrastText = "#FFFFFF",
            Secondary = "#B6CF48",
            SecondaryContrastText = "#1A2521",
            Tertiary = "#FF8560",
            TertiaryContrastText = "#1A2521",

            // Dark Mode Glassmorphic Overrides
            Background = "rgba(13, 19, 17, 0.8)",    // Near-black translucent canvas
            Surface = "rgba(21, 30, 27, 0.55)",     // Dark slate glass surfaces

            TextPrimary = "#ECF2F0",
            TextSecondary = "#A3B3AE",

            AppbarBackground = "#198754", // "rgba(25, 135, 84, 0.65)",
            AppbarText = "#FFFFFF",
            DrawerBackground = "rgba(21, 30, 27, 0.6)",
            DrawerText = "#ECF2F0",
            ActionDefault = "#A3B3AE"
        }
    };
} 

