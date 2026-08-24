using Fluxor;
using MudBlazor;
using Smbc.Risk.ReportingEngine.Domain.Shared.DataTransferObjects;
using Smbc.Risk.ReportingEngine.WebUI.Features.ReportMasters.Store;

namespace Smbc.Risk.ReportingEngine.WebUI.Features.DatabaseConnections.Store;

// State Definition
[FeatureState]
public record DatabaseConnectionState(bool IsLoading, List<DatabaseConnectionDto> Connections, string? ErrorMessage)
{
    public DatabaseConnectionState() : this(false, [], null) { }
}

// Actions
public record FetchDatabaseConnectionAction(bool IncludeInactive = false);
public record FetchDatabaseConnectionSuccessAction(List<DatabaseConnectionDto> Connections);

public record AddDatabaseConnectionAction(DatabaseConnectionDto Connection);
public record UpdateDatabaseConnectionAction(DatabaseConnectionDto Connection);
public record DeleteDatabaseConnectionAction(long? Id);

// Reducers
public static class DatabaseConnectionReducers
{
    [ReducerMethod]
    public static DatabaseConnectionState OnLoad(DatabaseConnectionState state, FetchDatabaseConnectionAction action)
    {
        return state with { IsLoading = true, ErrorMessage = null };
    }

    [ReducerMethod]
    public static DatabaseConnectionState OnSuccess(DatabaseConnectionState state, FetchDatabaseConnectionSuccessAction action)
    {
        return state with { IsLoading = false, Connections = action.Connections };
    }
}

public class DatabaseConnectionEffects(HttpClient httpClient, IConfiguration configuration, ISnackbar snackbar)
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly IConfiguration _configuration = configuration;
    private readonly ISnackbar _snackbar = snackbar;
    private string? apiEndpoint => $"{_configuration?.GetValue<string>("REPORT_MANAGEMENT_API")}DatabaseConenction";

    [EffectMethod]
    public async Task HandleFetch(FetchDatabaseConnectionAction action, IDispatcher dispatcher)
    {
        try
        {
            var connections = await _httpClient.GetFromJsonAsync<List<DatabaseConnectionDto>>(apiEndpoint) ?? [];
            dispatcher.Dispatch(new FetchDatabaseConnectionSuccessAction(connections));
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            // Dispatch a failure action instead of throwing so the circuit stays alive
            _snackbar.Add($"Unauthorized: Please log in again.: {ex.Message}", Severity.Error);
        }
        catch (Exception ex)
        {
            _snackbar.Add($"Unable to load database connections: {ex.Message}", Severity.Error);
        }
    }

    [EffectMethod]
    public async Task HandleAdd(AddDatabaseConnectionAction action, IDispatcher dispatcher)
    {
        try
        {
            if (action.Connection.Id <= 0)
            {
                await _httpClient.PostAsJsonAsync(apiEndpoint, action.Connection);
            }
            _snackbar.Add($"Report '{action.Connection.ConnectionName}' saved successfully.", Severity.Success);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            _snackbar.Add($"Unauthorized: Please log in again.: {ex.Message}", Severity.Error);
        }
        catch (Exception ex)
        {
            _snackbar.Add($"Unable to load reports: {ex.Message}", Severity.Error);
        }

        dispatcher.Dispatch(new FetchDatabaseConnectionAction());
    }

    [EffectMethod]
    public async Task HandleUpdate(UpdateDatabaseConnectionAction action, IDispatcher dispatcher)
    {
        var errorMessage = $"Unable to update Database Connection: {action.Connection.ConnectionName} [{action.Connection.Id}]";
        var successMessage = $"Database Connection '{action.Connection.ConnectionName}' saved successfully.";
        try
        {
            if (action.Connection.Id > 0)
            {
                await _httpClient.PutAsJsonAsync($"{apiEndpoint}/{action.Connection.Id}", action.Connection);
                _snackbar.Add(successMessage, Severity.Success);
            }
            else
            {
                _snackbar.Add(errorMessage, Severity.Error);
            }
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            _snackbar.Add($"Unauthorized: Please log in again.: {ex.Message}", Severity.Error);
        }
        catch (Exception ex)
        {
            _snackbar.Add($"Unable to update database connection: {ex.Message}", Severity.Error);
        }
        dispatcher.Dispatch(new FetchDatabaseConnectionAction());
    }

    [EffectMethod]
    public async Task HandleDelete(DeleteReportMasterAction action, IDispatcher dispatcher)
    {
        await _httpClient.DeleteAsync($"{apiEndpoint}/{action.Id}");
        dispatcher.Dispatch(new FetchDatabaseConnectionAction());
    }
}