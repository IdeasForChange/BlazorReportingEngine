using Fluxor;
using MudBlazor;
using Smbc.Risk.ReportingEngine.Domain.Shared.DataTransferObjects;
using Smbc.Risk.ReportingEngine.WebUI.Features.ReportTemplates.Store;

namespace Smbc.Risk.ReportingEngine.WebUI.Features.ReportMasters.Store;

// Represents the state of the Report Master feature
public record ReportMasterState(bool IsLoading, IEnumerable<ReportMasterDto> Reports, string? ErrorMessage);

// Represents the feature for managing Report Master state
public class ReportMasterFeature : Feature<ReportMasterState>
{
    public override string GetName() => nameof(ReportMasterFeature);

    protected override ReportMasterState GetInitialState()
    {
        return new ReportMasterState(
            IsLoading: false,
            Reports: [],
            ErrorMessage: null
        );
    }
}

// Actions for loading reports
public record LoadReportsAction;
public record LoadReportsSuccessAction(IEnumerable<ReportMasterDto> Reports);

// Reducers for handling state changes in response to actions
public static class ReportMasterReducers
{
    [ReducerMethod]
    public static ReportMasterState OnLoad(ReportMasterState state, LoadReportsAction action) 
    {
        return state with { IsLoading = true, ErrorMessage = null };
    }

    [ReducerMethod]
    public static ReportMasterState OnSuccess(ReportMasterState state, LoadReportsSuccessAction action) 
    {
        return state with { IsLoading = false, Reports = action.Reports }; 
    }
}

public class ReportMasterEffects(HttpClient httpClient, IConfiguration configuration, ISnackbar snackbar)
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly IConfiguration _configuration = configuration;
    private readonly ISnackbar _snackbar = snackbar;
    private string? apiEndpoint => $"{_configuration?.GetValue<string>("REPORT_MANAGEMENT_API")}ReportMaster";

    [EffectMethod]
    public async Task HandleLoadReports(LoadReportsAction action, IDispatcher dispatcher)
    {
        
        try
        {
            var reports = await _httpClient.GetFromJsonAsync<IEnumerable<ReportMasterDto>>(apiEndpoint);
            dispatcher.Dispatch(new LoadReportsSuccessAction(reports ?? []));
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            // Dispatch a failure action instead of throwing so the circuit stays alive
            _snackbar.Add($"Unauthorized: Please log in again.: {ex.Message}", Severity.Error);
        }
        catch (Exception ex)
        {
            _snackbar.Add($"Unable to load reports: {ex.Message}", Severity.Error);
        }
    }
}