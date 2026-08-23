using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Smbc.Risk.ReportingEngine.Domain.Services;
using Smbc.Risk.ReportingEngine.Domain.Shared.DataTransferObjects;

namespace Smbc.Risk.ReportingEngine.WebApi.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class ReportMasterController(IReportManagementService service) : ControllerBase
{
    private readonly IReportManagementService _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll(bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        var reports = await _service.GetAllReportsAsync(includeInactive, cancellationToken);
        return Ok(reports);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SaveReportMasterDto dto, CancellationToken cancellationToken)
    {
        var id = await _service.CreateReportAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetAll), new { id }, id);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] ReportMasterDto dto, CancellationToken cancellationToken)
    {
        //await _service.UpdateReportAsync(id, dto, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        //await _service.DeleteReportAsync(id, cancellationToken);
        return NoContent();
    }
}
