using Fluxor;
using MudBlazor;
using Smbc.Risk.ReportingEngine.Domain.Shared.DataTransferObjects;
using Smbc.Risk.ReportingEngine.Domain.Shared.Enums;
using Smbc.Risk.ReportingEngine.WebUI.Features.ReportMasters.Store;

namespace Smbc.Risk.ReportingEngine.WebUI.Features.ReportQueues.Store;

// States
[FeatureState]
public record ReportQueueState(bool IsLoading, IEnumerable<ReportRunnerQueueDto> QueueItems, string SelectedFilter)
{
    public ReportQueueState() : this(false, [], null) { }
}

// Actions
public record FetchReportQueueAction(string Filter);
public record FetchReportQueueSuccessAction(IEnumerable<ReportRunnerQueueDto> Items);
public record RunReportAction(EnqueueReportRequestDto ReportRequest);
public record CancelQueueItemAction(long QueueItemId);

// Reducer
public static class ReportQueueReducers
{
    [ReducerMethod]
    public static ReportQueueState ReduceFetchReportQueueAction(ReportQueueState state, FetchReportQueueAction action) =>
        state with { IsLoading = true, SelectedFilter = action.Filter };

    [ReducerMethod]
    public static ReportQueueState ReduceFetchReportQueueSuccessAction(ReportQueueState state, FetchReportQueueSuccessAction action) =>
        state with { IsLoading = false, QueueItems = action.Items };
}

// Effects
public class ReportQueueEffects(HttpClient httpClient, IConfiguration configuration, ISnackbar snackbar)
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly IConfiguration _configuration = configuration;
    private readonly ISnackbar _snackbar = snackbar;
    private string? apiEndpoint => $"{_configuration?.GetValue<string>("REPORT_MANAGEMENT_API")}Reports";

    [EffectMethod]
    public async Task HandleFetchReportQueue(FetchReportQueueAction action, IDispatcher dispatcher)
    {
        try
        {
            var reports = await _httpClient.GetFromJsonAsync<IEnumerable<ReportRunnerQueueDto>>($"{apiEndpoint}/queue?filter={action.Filter}");
            dispatcher.Dispatch(new FetchReportQueueSuccessAction(reports ?? []));
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            // Dispatch a failure action instead of throwing so the circuit stays alive
            _snackbar.Add($"Unauthorized: Please log in again.: {ex.Message}", Severity.Error);
        }
        catch (Exception ex)
        {
            _snackbar.Add($"Unable to load report queues: {ex.Message}", Severity.Error);
        }
    }

    [EffectMethod]
    public async Task HandleRunReport(RunReportAction action, IDispatcher dispatcher)
    {
        try
        {
            await _httpClient.PostAsJsonAsync($"{apiEndpoint}/enqueue", action.ReportRequest);
            _snackbar.Add($"Report enqueued successfully.", Severity.Success);

            dispatcher.Dispatch(new FetchReportQueueAction("Today"));
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            // Dispatch a failure action instead of throwing so the circuit stays alive
            _snackbar.Add($"Unauthorized: Please log in again.: {ex.Message}", Severity.Error);
        }
        catch (Exception ex)
        {
            _snackbar.Add($"Unable to load report queues: {ex.Message}", Severity.Error);
        }
    }

    [EffectMethod]
    public async Task HandleCancelQueueItem(CancelQueueItemAction action, IDispatcher dispatcher)
    {
        try
        {
            var reports = await _httpClient.GetFromJsonAsync<IEnumerable<ReportRunnerQueueDto>>(apiEndpoint);
            dispatcher.Dispatch(new FetchReportQueueSuccessAction(reports ?? []));
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            // Dispatch a failure action instead of throwing so the circuit stays alive
            _snackbar.Add($"Unauthorized: Please log in again.: {ex.Message}", Severity.Error);
        }
        catch (Exception ex)
        {
            _snackbar.Add($"Unable to load report queues: {ex.Message}", Severity.Error);
        }
    }
}