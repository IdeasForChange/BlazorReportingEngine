using Fluxor;

namespace Smbc.Risk.ReportingEngine.WebUI.Features.Authorisations.Store;

public record UserState
{
    public bool IsLoading { get; init; }
    public string? AdAccount { get; init; }
    public List<string> Permissions { get; init; } = [];
    public string? ErrorMessage { get; init; }
}

// FEATURE
public class UserStateFeature : Feature<UserState>
{
    public override string GetName() => nameof(UserState);
    protected override UserState GetInitialState()
    {
        return new UserState
        {
            IsLoading = false,
            AdAccount = null,
            Permissions = [],
            ErrorMessage = null
        };
    }
}

// ACTIONS
public record FetchPermissionsAction(string AdAccount);
public record FetchPermissionsSuccessAction(string AdAccount, List<string> Permissions);
public record FetchPermissionsFailureAction(string ErrorMessage);

// REDUCERS
public static class UserReducers
{
    [ReducerMethod]
    public static UserState OnFetchPermissions(UserState state, FetchPermissionsAction action)
    {
        return state with
        {
            IsLoading = true,
            AdAccount = action.AdAccount,
            Permissions = state.Permissions,
            ErrorMessage = null
        };
    }

    [ReducerMethod]
    public static UserState OnFetchPermissionsSuccess(UserState state, FetchPermissionsSuccessAction action)
    {
        return state with
        {
            IsLoading = false,
            AdAccount = action.AdAccount,
            Permissions = action.Permissions,
            ErrorMessage = null
        };
    }

    [ReducerMethod]
    public static UserState OnFetchPermissionsFailure(UserState state, FetchPermissionsFailureAction action)
    {
        return state with
        {
            IsLoading = false,
            AdAccount = state.AdAccount,
            Permissions = [],
            ErrorMessage = action.ErrorMessage
        };
    }
}

// EFFECTS
//public class UserEffects(IPermissionApiService permissionService)
//{
//    private readonly IPermissionApiService _permissionService = permissionService;

//    [EffectMethod]
//    public async Task HandleFetchPermissionsAction(FetchPermissionsAction action, IDispatcher dispatcher)
//    {
//        try
//        {
//            var permissions = await _permissionService.GetPermissionsAsync(action.AdAccount);
//            dispatcher.Dispatch(new FetchPermissionsSuccessAction(action.AdAccount, permissions));
//        }
//        catch (Exception ex)
//        {
//            dispatcher.Dispatch(new FetchPermissionsFailureAction(ex.Message));
//        }
//    }
//}