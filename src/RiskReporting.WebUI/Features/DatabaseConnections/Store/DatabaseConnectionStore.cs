using Fluxor;
using MudBlazor;
using Smbc.Risk.ReportingEngine.Domain.Shared.DataTransferObjects;
using Smbc.Risk.ReportingEngine.WebUI.Features.Notifications.Store;
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

public class DatabaseConnectionEffects(
    HttpClient httpClient,
    IConfiguration configuration,
    ISnackbar snackbar)
{
    private string? apiEndpoint => $"{configuration?.GetValue<string>("REPORT_MANAGEMENT_API")}DatabaseConnection";

    [EffectMethod]
    public async Task HandleFetch(FetchDatabaseConnectionAction action, IDispatcher dispatcher)
    {
        try
        {
            var connections = await httpClient.GetFromJsonAsync<List<DatabaseConnectionDto>>(apiEndpoint) ?? [];

            dispatcher.Dispatch(new FetchDatabaseConnectionSuccessAction(connections));
            dispatcher.Dispatch(new ShowInfoAction($"Connections Loaded Successfuly.", "Database Connection"));

        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            // Dispatch a failure action instead of throwing so the circuit stays alive
            dispatcher.Dispatch(new ShowInfoAction($"Unauthorized: Please log in again.: {ex.Message}", "Database Connection"));
            snackbar.Add($"Unauthorized: Please log in again.: {ex.Message}", Severity.Error);
        }
        catch (Exception ex)
        {
            snackbar.Add($"Unable to load database connections: {ex.Message}", Severity.Error);
        }
    }

    [EffectMethod]
    public async Task HandleAdd(AddDatabaseConnectionAction action, IDispatcher dispatcher)
    {
        try
        {
            if (action.Connection.Id == null)
            {
                await httpClient.PostAsJsonAsync(apiEndpoint, action.Connection);
                snackbar.Add($"Database Connection '{action.Connection.ConnectionName}' saved successfully.", Severity.Success);
            }
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            snackbar.Add($"Unauthorized: Please log in again.: {ex.Message}", Severity.Error);
        }
        catch (Exception ex)
        {
            snackbar.Add($"Unable to load database connections: {ex.Message}", Severity.Error);
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
                await httpClient.PutAsJsonAsync($"{apiEndpoint}/{action.Connection.Id}", action.Connection);
                snackbar.Add(successMessage, Severity.Success);
            }
            else
            {
                snackbar.Add(errorMessage, Severity.Error);
            }
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            snackbar.Add($"Unauthorized: Please log in again.: {ex.Message}", Severity.Error);
        }
        catch (Exception ex)
        {
            snackbar.Add($"Unable to update database connection: {ex.Message}", Severity.Error);
        }
        dispatcher.Dispatch(new FetchDatabaseConnectionAction());
    }

    [EffectMethod]
    public async Task HandleDelete(DeleteReportMasterAction action, IDispatcher dispatcher)
    {
        await httpClient.DeleteAsync($"{apiEndpoint}/{action.Id}");
        dispatcher.Dispatch(new FetchDatabaseConnectionAction());
    }
}