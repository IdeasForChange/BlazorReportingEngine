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

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken = default)
    {
        var result = await _service.GetByIdAsync(id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SaveReportMasterDto dto, CancellationToken cancellationToken)
    {
        var id = await _service.CreateReportAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetAll), new { id }, id);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] ReportMasterDto dto, CancellationToken cancellationToken)
    {
        if (id != dto.Id)
        {
            return BadRequest("Mismatched ID");
        }

        // Update the report using the service.
        // TODO: Pass the current user or any other necessary information.
        var updatedReport = await _service.UpdateAsync(dto, "System", cancellationToken);
        return Ok(updatedReport);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, [FromQuery] bool hardDelete = false, CancellationToken cancellationToken = default)
    {
        await _service.DeleteAsync(id, hardDelete, cancellationToken);
        return NoContent();
    }
}
