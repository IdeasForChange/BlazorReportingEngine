using Fluxor;

namespace Smbc.Risk.ReportingEngine.WebUI.Features.Themes.Store;

public record NotificationItem(int Id, string Title, string Time, bool IsRead);


[FeatureState]
public record AppState
{
    public bool IsSidebarOpen { get; init; } = true;
    public bool IsDarkMode { get; init; } = false;
    public string UserName { get; init; } = "Harry Held";
    public string UserEmail { get; init; } = "harryheld@smbcgroup.com";
    public string UserAvatarUrl { get; init; } = "https://i.pravatar.cc/150?img=33";
    public List<NotificationItem> Notifications { get; init; } = new()
    {
        new(1, "New report generated: Risk VaR Report", "10m ago", false),
        new(2, "Makret Risk MARS Batch completed.", "1h ago", false),
        new(3, "Final Batch run by Support Team", "3h ago", true)
    };

    public int UnreadNotificationCount => Notifications.Count(n => !n.IsRead);

    private AppState() { } // Required for Fluxor initial state

    public AppState(bool isSidebarOpen, bool isDarkMode)
    {
        IsSidebarOpen = isSidebarOpen;
        IsDarkMode = isDarkMode;
    }
}


// Actions
public record ToggleSidebarAction;
public record ToggleThemeAction;
public record MarkAllNotificationsReadAction;

// Reducers
public static class AppReducers
{
    [ReducerMethod]
    public static AppState ReduceToggleSidebarAction(AppState state, ToggleSidebarAction action)
        => state with { IsSidebarOpen = !state.IsSidebarOpen };

    [ReducerMethod]
    public static AppState ReduceToggleThemeAction(AppState state, ToggleThemeAction action)
        => state with { IsDarkMode = !state.IsDarkMode };

    [ReducerMethod]
    public static AppState ReduceMarkAllNotificationsReadAction(AppState state, MarkAllNotificationsReadAction action)
    {
        var updatedNotifications = state.Notifications.Select(n => n with { IsRead = true }).ToList();
        return state with { Notifications = updatedNotifications };
    }
}