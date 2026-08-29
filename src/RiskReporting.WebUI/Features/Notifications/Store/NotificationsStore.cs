using Fluxor;
using MudBlazor;

namespace Smbc.Risk.ReportingEngine.WebUI.Features.Notifications.Store;

public record Notification(
    Guid Id,
    string Title,
    string Message,
    DateTime CreatedAt,
    bool IsRead);

[FeatureState]
public record NotificationState
{
    public IReadOnlyList<Notification> Items { get; init; } = [];
    public bool IsLoading { get; init; }

    public int UnreadCount =>  Items.Count(x => !x.IsRead);
}

public record LoadNotificationsAction;
public record NotificationsLoadedAction(IReadOnlyList<Notification> Notifications);
public record MarkNotificationReadAction(Guid Id);
public record NotificationMarkedReadAction(Guid Id);
public record ShowInfoAction(string Message, string? Title = null, Action? ClickAction = null);
public record ShowSuccessAction(string Message, string? Title = null, Action? ClickAction = null);
public record ShowWarningAction(string Message, string? Title = null, Action? ClickAction = null);
public record ShowErrorAction(string Message, string? Title = null, Action? ClickAction = null);


public static class NotificationReducers
{
    [ReducerMethod]
    public static NotificationState ReduceLoad(NotificationState state, LoadNotificationsAction action)
    {
        return state with
        {
            IsLoading = true
        };
    }

    [ReducerMethod]
    public static NotificationState ReduceLoaded(NotificationState state, NotificationsLoadedAction action)
    {
        return state with
        {
            Items = action.Notifications,
            IsLoading = false
        };
    }

    [ReducerMethod]
    public static NotificationState ReduceMarkedRead(NotificationState state, NotificationMarkedReadAction action)
    {
        return state with
        {
            Items = state.Items
                .Select(n => n.Id == action.Id
                    ? n with { IsRead = true }
                    : n)
                .ToList()
        };
    }
}

public class NotificationEffects(ISnackbar snackbar, IState<NotificationState> notificationState)
{
    private readonly ISnackbar _snackbar = snackbar;
    private readonly IState<NotificationState> _notificationState = notificationState;

    [EffectMethod]
    public async Task LoadNotifications(LoadNotificationsAction action, IDispatcher dispatcher)
    {
        dispatcher.Dispatch(new LoadNotificationsAction());
    }

    [EffectMethod]
    public async Task MarkAsRead(MarkNotificationReadAction action, IDispatcher dispatcher)
    {
        var notifications = _notificationState.Value.Items.ToList(); 
        var notification = notifications.FirstOrDefault(n => n.Id == action.Id);
        if (notification == null)
            return;

        notifications.Remove(notification);

        dispatcher.Dispatch(new NotificationsLoadedAction(notifications));
    }

    [EffectMethod]
    public async Task ShowInfo(ShowInfoAction action, IDispatcher dispatcher)
    {
        var notifications = _notificationState.Value.Items.ToList();
        notifications.Add(new(Guid.NewGuid(), action.Title ?? "New Message", action.Message, DateTime.UtcNow, false));

        dispatcher.Dispatch(new NotificationsLoadedAction(notifications));
    }
}


