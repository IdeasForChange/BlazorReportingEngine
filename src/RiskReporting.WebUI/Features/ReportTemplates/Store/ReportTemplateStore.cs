using Fluxor;
using MudBlazor;
using Smbc.Risk.ReportingEngine.Domain.Shared.DataTransferObjects;
using static System.Net.WebRequestMethods;

namespace Smbc.Risk.ReportingEngine.WebUI.Features.ReportTemplates.Store;

// STATE
public record ReportTemplateState(
    bool IsLoading,
    bool IsSaving,
    IEnumerable<ReportTemplateDto> Templates,
    ReportTemplateDto? SelectedReportTemplate,
    string? Error
);

// FEATURE
public class ReportTemplateFeature : Feature<ReportTemplateState>
{
    public override string GetName() => nameof(ReportTemplateFeature);

    protected override ReportTemplateState GetInitialState()
    {
        return new ReportTemplateState(
            IsLoading: false,
            IsSaving: false,
            Templates: [],
            SelectedReportTemplate: null,
            Error: null
        );
    }
}
       

// ACTIONS
public record LoadReportsAction;
public record LoadReportsSuccessAction(IEnumerable<ReportTemplateDto> Templates);
public record LoadReportsFailedAction(string ErrorMessage);
public record SelectReportAction(long Id);
public record SelectReportSuccessAction(ReportTemplateDto ReportTemplate);

// Create Actions
public record CreateTemplateAction(CreateReportTemplateRequest Template);
public record CreateTemplateSuccessAction(ReportTemplateDto Template);
public record CreateTemplateFailureAction(string Error);


// REDUCERS
public static class ReportReducers
{
    [ReducerMethod]
    public static ReportTemplateState ReduceLoadReports(ReportTemplateState state, LoadReportsAction _) =>
        state with { IsLoading = true };

    [ReducerMethod]
    public static ReportTemplateState ReduceLoadReportsSuccess(ReportTemplateState state, LoadReportsSuccessAction action)
    {
        return state with { 
            IsLoading = false, 
            Templates = action.Templates 
        };
    }       

    [ReducerMethod]
    public static ReportTemplateState ReduceSelectReportSuccess(ReportTemplateState state, SelectReportSuccessAction action) =>
        state with { SelectedReportTemplate = action.ReportTemplate };

    // --- Create Reducers ---
    [ReducerMethod]
    public static ReportTemplateState OnCreate(ReportTemplateState state, CreateTemplateAction _) =>
        state with { IsSaving = true, Error = null };

    [ReducerMethod]
    public static ReportTemplateState OnCreateSuccess(ReportTemplateState state, CreateTemplateSuccessAction action) =>
        state with { IsSaving = false, Templates = state.Templates.Append(action.Template) };

    [ReducerMethod]
    public static ReportTemplateState OnCreateFailure(ReportTemplateState state, CreateTemplateFailureAction action) =>
        state with { IsSaving = false, Error = action.Error };
}

// EFFECTS
public class ReportEffects(HttpClient httpClient, ISnackbar snackbar)
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly ISnackbar _snackbar = snackbar;
    private const string ApiEndpoint = "https://localhost:7105/api/ReportTemplates";


    [EffectMethod]
    public async Task HandleLoadReports(LoadReportsAction action, IDispatcher dispatcher)
    {
        try
        {
            var reports = await _httpClient.GetFromJsonAsync<List<ReportTemplateDto>>($"{ApiEndpoint}");

            dispatcher.Dispatch(new LoadReportsSuccessAction(reports!));
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            // Dispatch a failure action instead of throwing so the circuit stays alive
            dispatcher.Dispatch(new LoadReportsFailedAction("Unauthorized: Please log in again."));
        }
        catch (Exception ex)
        {
            dispatcher.Dispatch(new LoadReportsFailedAction(ex.Message));
        }
    }

    [EffectMethod]
    public async Task HandleCreate(CreateTemplateAction action, IDispatcher dispatcher)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(ApiEndpoint, action.Template);
            if (response.IsSuccessStatusCode)
            {
                var created = await response.Content.ReadFromJsonAsync<ReportTemplateDto>();
                if (created != null)
                {
                    _snackbar.Add("Report template created successfully!", Severity.Success);
                    dispatcher.Dispatch(new CreateTemplateSuccessAction(created));
                    return;
                }
            }

            _snackbar.Add("Failed to create report template.", Severity.Error);
            dispatcher.Dispatch(new CreateTemplateFailureAction("Failed to create template."));
        }
        catch (Exception ex)
        {
            _snackbar.Add($"Error creating template: {ex.Message}", Severity.Error);
            dispatcher.Dispatch(new CreateTemplateFailureAction(ex.Message));
        }
    }

    [EffectMethod]
    public async Task HandleSelectReport(SelectReportAction action, IDispatcher dispatcher)
    {
        var report = await _httpClient.GetFromJsonAsync<ReportTemplateDto>($"{ApiEndpoint}/{action.Id}");
        if (report != null) dispatcher.Dispatch(new SelectReportSuccessAction(report));
    }
}