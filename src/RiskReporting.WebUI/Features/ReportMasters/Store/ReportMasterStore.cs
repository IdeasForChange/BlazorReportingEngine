using Fluxor;
using MudBlazor;
using Smbc.Risk.ReportingEngine.Domain.Shared.DataTransferObjects;
using static System.Net.WebRequestMethods;

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
public record FetchReportMasterAction(bool IncludeInactive = false);
public record FetchReportMasterSuccessAction(IEnumerable<ReportMasterDto> Reports);

public record SaveReportMasterAction(SaveReportMasterDto ReportMaster);
public record UpdateReportMasterAction(ReportMasterDto ReportMaster);
public record DeleteReportMasterAction(long Id);

// Reducers for handling state changes in response to actions
public static class ReportMasterReducers
{
    [ReducerMethod]
    public static ReportMasterState OnLoad(ReportMasterState state, FetchReportMasterAction action) 
    {
        return state with { IsLoading = true, ErrorMessage = null };
    }

    [ReducerMethod]
    public static ReportMasterState OnSuccess(ReportMasterState state, FetchReportMasterSuccessAction action) 
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
    public async Task HandleLoadReports(FetchReportMasterAction action, IDispatcher dispatcher)
    {
        try
        {
            var reports = await _httpClient.GetFromJsonAsync<IEnumerable<ReportMasterDto>>(apiEndpoint);
            dispatcher.Dispatch(new FetchReportMasterSuccessAction(reports ?? []));
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

    [EffectMethod]
    public async Task HandleSave(SaveReportMasterAction action, IDispatcher dispatcher)
    {
        try
        {
            if (action.ReportMaster.Id.HasValue && action.ReportMaster.Id > 0)
            {
                await _httpClient.PutAsJsonAsync($"{apiEndpoint}/{action.ReportMaster.Id}", action.ReportMaster);
            }
            else
            {
                await _httpClient.PostAsJsonAsync(apiEndpoint, action.ReportMaster);
            }
            _snackbar.Add($"Report '{action.ReportMaster.Name}' saved successfully.", Severity.Success);
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

        dispatcher.Dispatch(new FetchReportMasterAction());
    }

    [EffectMethod]
    public async Task HandleSave(UpdateReportMasterAction action, IDispatcher dispatcher)
    {
        try
        {
            if (action.ReportMaster.Id > 0)
            {
                await _httpClient.PutAsJsonAsync($"{apiEndpoint}/{action.ReportMaster.Id}", action.ReportMaster);
                _snackbar.Add($"Report '{action.ReportMaster.Name}' saved successfully.", Severity.Success);
            }
            else
            {
                _snackbar.Add($"Unable to update master report: {action.ReportMaster.Name} [{action.ReportMaster.Id}]", Severity.Error);
            }
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
        dispatcher.Dispatch(new FetchReportMasterAction());
    }

    [EffectMethod]
    public async Task HandleDelete(DeleteReportMasterAction action, IDispatcher dispatcher)
    {
        await _httpClient.DeleteAsync($"api/ReportMaster/{action.Id}");
        dispatcher.Dispatch(new FetchReportMasterAction());
    }
}