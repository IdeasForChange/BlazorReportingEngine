using Fluxor;

namespace Smbc.RiskReporting.WebUI.Features.Themes.Store;

// STATE 
public record ThemeState 
{
    public bool IsDarkMode { get; init; }
}

// FEATURE
public class ThemeFeature : Feature<ThemeState>
{
    public override string GetName() => nameof(ThemeFeature);

    protected override ThemeState GetInitialState()
    {
        return new ThemeState
        {
            IsDarkMode = false
        };
    }
}

// ACTIONS
public record LoadDarkModeAction();
public record ToggleDarkModeAction();
public record SetDarkModeAction(bool IsDarkMode);

// REDUCERS 
public static class ThemeReducers 
{
    [ReducerMethod]
    public static ThemeState OnSetDarkMode(ThemeState state, SetDarkModeAction action) 
    {
        return state with
        {
            IsDarkMode = action.IsDarkMode
        };
    }
}

//public class ThemeEffects(ApplicationSettingRepository appSettingRepo)
//{
//    private const string APP_SETTING_IS_DARK_MODE = "IsDarkMode";
//    private readonly ApplicationSettingRepository _appSettingRepo = appSettingRepo;

//    [EffectMethod(typeof(LoadDarkModeAction))]
//    public async Task OnLoadDarkMode(IDispatcher dispatcher) 
//    {
//        var appSetting = await _appSettingRepo.GetOrAdd(APP_SETTING_IS_DARK_MODE, "false");
//        dispatcher.Dispatch(new SetDarkModeAction(bool.Parse(appSetting.Value)));
//    }

//    [EffectMethod(typeof(ToggleDarkModeAction))]
//    public async Task OnToggleDarkModeAction(IDispatcher dispatcher)
//    {
//        var appSetting = await _appSettingRepo.Toggle(APP_SETTING_IS_DARK_MODE);
//        dispatcher.Dispatch(new SetDarkModeAction(bool.Parse(appSetting.Value)));
//    }
//}
