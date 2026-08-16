using Fluxor;
using Smbc.Risk.ReportingEngine.Domain.Shared.DataTransferObjects;

namespace Smbc.Risk.ReportingEngine.WebUI.Features.Reports.Store;

// State Definition
public record ReportState(
    bool IsLoading,
    List<ReportTemplateDto> Templates,
    List<ReportQueueDto> QueueItems,
    string? ErrorMessage
);

public class ReportFeature : Feature<ReportState>
{
    public override string GetName() => "Reports";
    protected override ReportState GetInitialState() => new(false, [], [], null);
}

// Actions
public record FetchTemplatesAction();
public record FetchTemplatesSuccessAction(List<ReportTemplateDto> Templates);
public record FetchQueueAction();
public record FetchQueueSuccessAction(List<ReportQueueDto> QueueItems);
public record EnqueueJobAction(long TemplateId, Dictionary<string, string> Parameters);
public record KillJobAction(long QueueId);
public record ReportFailureAction(string ErrorMessage);

// Reducers
public static class ReportReducers
{
    [ReducerMethod]
    public static ReportState OnFetchTemplates(ReportState state, FetchTemplatesAction action) =>
        state with { IsLoading = true };

    [ReducerMethod]
    public static ReportState OnFetchTemplatesSuccess(ReportState state, FetchTemplatesSuccessAction action) =>
        state with { IsLoading = false, Templates = action.Templates };

    [ReducerMethod]
    public static ReportState OnFetchQueueSuccess(ReportState state, FetchQueueSuccessAction action) =>
        state with { QueueItems = action.QueueItems };

    [ReducerMethod]
    public static ReportState OnFailure(ReportState state, ReportFailureAction action) =>
        state with { IsLoading = false, ErrorMessage = action.ErrorMessage };
}

public class ReportEffects(HttpClient http)
{
    private readonly HttpClient _http = http;

    [EffectMethod]
    public async Task HandleFetchTemplates(FetchTemplatesAction action, IDispatcher dispatcher)
    {
        try
        {
            var templates = await _http.GetFromJsonAsync<List<ReportTemplateDto>>("api/reports/templates");
            dispatcher.Dispatch(new FetchTemplatesSuccessAction(templates ?? new()));
        }
        catch (Exception ex)
        {
            dispatcher.Dispatch(new ReportFailureAction(ex.Message));
        }
    }

    [EffectMethod]
    public async Task HandleFetchQueue(FetchQueueAction action, IDispatcher dispatcher)
    {
        try
        {
            var queue = await _http.GetFromJsonAsync<List<ReportQueueDto>>("api/reports/queue");
            dispatcher.Dispatch(new FetchQueueSuccessAction(queue ?? new()));
        }
        catch (Exception ex)
        {
            dispatcher.Dispatch(new ReportFailureAction(ex.Message));
        }
    }

    [EffectMethod]
    public async Task HandleEnqueueJob(EnqueueJobAction action, IDispatcher dispatcher)
    {
        try
        {
            var dto = new EnqueueReportRequestDto(action.TemplateId, action.Parameters);
            await _http.PostAsJsonAsync("api/reports/enqueue", dto);
            dispatcher.Dispatch(new FetchQueueAction());
        }
        catch (Exception ex)
        {
            dispatcher.Dispatch(new ReportFailureAction(ex.Message));
        }
    }

    [EffectMethod]
    public async Task HandleKillJob(KillJobAction action, IDispatcher dispatcher)
    {
        try
        {
            await _http.PostAsync($"api/reports/kill/{action.QueueId}", null);
            dispatcher.Dispatch(new FetchQueueAction());
        }
        catch (Exception ex)
        {
            dispatcher.Dispatch(new ReportFailureAction(ex.Message));
        }
    }
}