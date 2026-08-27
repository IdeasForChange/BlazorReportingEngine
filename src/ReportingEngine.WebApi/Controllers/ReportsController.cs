using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Smbc.Risk.ReportingEngine.Domain.Services;
using Smbc.Risk.ReportingEngine.Domain.Shared.DataTransferObjects;

namespace Smbc.Risk.ReportingEngine.WebApi.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class ReportsController(IReportManagementService reportService) : ControllerBase
{
    private readonly IReportManagementService _reportService = reportService;

    [HttpGet("masters/{masterId:long}/parameters")]
    public async Task<ActionResult<IEnumerable<ReportParameterDto>>> GetParameters(long masterId)
    {
        var result = await _reportService.GetParametersByMasterIdAsync(masterId);
        return Ok(result);
    }

    [HttpPost("enqueue")]
    public async Task<ActionResult<ReportRunnerQueueDto>> Enqueue([FromBody] EnqueueReportRequestDto request)
    {
        var result = await _reportService.EnqueueReportJobAsync(request);
        return Ok(result);
    }

    [HttpGet("queue")]
    public async Task<ActionResult<IEnumerable<ReportRunnerQueueDto>>> GetQueue([FromQuery] string filter = "Today")
    {
        var result = await _reportService.GetQueueItemsAsync(filter);
        return Ok(result);
    }

    [HttpPost("queue/{queueId:long}/cancel")]
    public async Task<IActionResult> CancelJob(long queueId)
    {
        await _reportService.CancelQueueItemAsync(queueId);
        return NoContent();
    }
}
