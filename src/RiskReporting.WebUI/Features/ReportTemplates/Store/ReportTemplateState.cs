using Fluxor;
using MudBlazor;
using Smbc.Risk.ReportingEngine.Domain.Shared.DataTransferObjects;

namespace Smbc.Risk.ReportingEngine.WebUI.Features.ReportTemplates.Store;

// STATE
public record ReportTemplateState(
    bool IsLoading,
    bool IsSaving,
    IEnumerable<ReportTemplateDto> Templates,
    string? Error
);

// FEATURE
public class ReportTemplateFeature : Feature<ReportTemplateState>
{
    public override string GetName() => "ReportTemplates";

    protected override ReportTemplateState GetInitialState() =>
        new(IsLoading: false, IsSaving: false, Templates: Array.Empty<ReportTemplateDto>(), Error: null);
}

// ACTIONS
// Fetch Actions
public record FetchTemplatesAction;
public record FetchTemplatesSuccessAction(IEnumerable<ReportTemplateDto> Templates);
public record FetchTemplatesFailureAction(string Error);

// Create Actions
public record CreateTemplateAction(ReportTemplateDto Template);
public record CreateTemplateSuccessAction(ReportTemplateDto Template);
public record CreateTemplateFailureAction(string Error);

// Update Actions
public record UpdateTemplateAction(ReportTemplateDto Template);
public record UpdateTemplateSuccessAction(ReportTemplateDto Template);
public record UpdateTemplateFailureAction(string Error);

// Delete Actions
public record DeleteTemplateAction(long Id);
public record DeleteTemplateSuccessAction(long Id);
public record DeleteTemplateFailureAction(string Error);

// REDUCERS
public static class ReportTemplateReducers
{
    [ReducerMethod]
    public static ReportTemplateState OnFetch(ReportTemplateState state, FetchTemplatesAction _) =>
            state with { IsLoading = true, Error = null };

    [ReducerMethod]
    public static ReportTemplateState OnFetchSuccess(ReportTemplateState state, FetchTemplatesSuccessAction action) =>
        state with { IsLoading = false, Templates = action.Templates };

    [ReducerMethod]
    public static ReportTemplateState OnFetchFailure(ReportTemplateState state, FetchTemplatesFailureAction action) =>
        state with { IsLoading = false, Error = action.Error };

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

    // --- Update Reducers ---
    [ReducerMethod]
    public static ReportTemplateState OnUpdate(ReportTemplateState state, UpdateTemplateAction _) =>
        state with { IsSaving = true, Error = null };

    [ReducerMethod]
    public static ReportTemplateState OnUpdateSuccess(ReportTemplateState state, UpdateTemplateSuccessAction action) =>
        state with
        {
            IsSaving = false,
            Templates = state.Templates.Select(t => t.Id == action.Template.Id ? action.Template : t).ToList()
        };

    [ReducerMethod]
    public static ReportTemplateState OnUpdateFailure(ReportTemplateState state, UpdateTemplateFailureAction action) =>
        state with { IsSaving = false, Error = action.Error };

    // --- Delete Reducers ---
    [ReducerMethod]
    public static ReportTemplateState OnDelete(ReportTemplateState state, DeleteTemplateAction _) =>
        state with { IsSaving = true, Error = null };

    [ReducerMethod]
    public static ReportTemplateState OnDeleteSuccess(ReportTemplateState state, DeleteTemplateSuccessAction action) =>
        state with
        {
            IsSaving = false,
            Templates = state.Templates.Where(t => t.Id != action.Id).ToList()
        };

    [ReducerMethod]
    public static ReportTemplateState OnDeleteFailure(ReportTemplateState state, DeleteTemplateFailureAction action) =>
        state with { IsSaving = false, Error = action.Error };
}

// EFFECTS
public class ReportTemplateEffects(HttpClient http, ISnackbar snackbar)
{
    private readonly HttpClient _http = http;
    private readonly ISnackbar _snackbar = snackbar;
    private const string ApiEndpoint = "api/ReportTemplates";

    [EffectMethod]
    public async Task HandleFetch(FetchTemplatesAction action, IDispatcher dispatcher)
    {
        try
        {
            var result = await _http.GetFromJsonAsync<IEnumerable<ReportTemplateDto>>(ApiEndpoint);
            dispatcher.Dispatch(new FetchTemplatesSuccessAction(result ?? Array.Empty<ReportTemplateDto>()));
        }
        catch (Exception ex)
        {
            _snackbar.Add($"Failed to load templates: {ex.Message}", Severity.Error);
            dispatcher.Dispatch(new FetchTemplatesFailureAction(ex.Message));
        }
    }

    [EffectMethod]
    public async Task HandleCreate(CreateTemplateAction action, IDispatcher dispatcher)
    {
        try
        {
            var response = await _http.PostAsJsonAsync(ApiEndpoint, action.Template);
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
    public async Task HandleUpdate(UpdateTemplateAction action, IDispatcher dispatcher)
    {
        try
        {
            var response = await _http.PutAsJsonAsync($"{ApiEndpoint}/{action.Template.Id}", action.Template);
            if (response.IsSuccessStatusCode)
            {
                _snackbar.Add("Report template updated successfully!", Severity.Success);
                dispatcher.Dispatch(new UpdateTemplateSuccessAction(action.Template));
            }
            else
            {
                _snackbar.Add("Failed to update report template.", Severity.Error);
                dispatcher.Dispatch(new UpdateTemplateFailureAction("Failed to update template."));
            }
        }
        catch (Exception ex)
        {
            _snackbar.Add($"Error updating template: {ex.Message}", Severity.Error);
            dispatcher.Dispatch(new UpdateTemplateFailureAction(ex.Message));
        }
    }

    [EffectMethod]
    public async Task HandleDelete(DeleteTemplateAction action, IDispatcher dispatcher)
    {
        try
        {
            var response = await _http.DeleteAsync($"{ApiEndpoint}/{action.Id}");
            if (response.IsSuccessStatusCode)
            {
                _snackbar.Add("Report template deleted.", Severity.Warning);
                dispatcher.Dispatch(new DeleteTemplateSuccessAction(action.Id));
            }
            else
            {
                _snackbar.Add("Failed to delete report template.", Severity.Error);
                dispatcher.Dispatch(new DeleteTemplateFailureAction("Failed to delete template."));
            }
        }
        catch (Exception ex)
        {
            _snackbar.Add($"Error deleting template: {ex.Message}", Severity.Error);
            dispatcher.Dispatch(new DeleteTemplateFailureAction(ex.Message));
        }
    }
}