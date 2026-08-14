using Microsoft.AspNetCore.Mvc;
using Smbc.Risk.ReportingEngine.Application.DataTransferObjects;
using Smbc.Risk.ReportingEngine.Domain.Services;

namespace Smbc.Risk.ReportingEngine.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SystemParameterTypesController(ISystemParameterTypeService service) : ControllerBase
{
    private readonly ISystemParameterTypeService _service = service;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SystemParameterTypeDto>>> GetAll(CancellationToken ct)
    {
        return Ok(await _service.GetAll(ct));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<SystemParameterTypeDto>> GetById(long id, CancellationToken ct)
    {
        var result = await _service.GetById(id, ct);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpGet("count")]
    public async Task<ActionResult<int>> GetCount(CancellationToken ct) => Ok(await _service.Count(ct));

    [HttpGet("exists/{id:long}")]
    public async Task<ActionResult<bool>> Exists(long id, CancellationToken ct) => Ok(await _service.Exists(id, ct));

    [HttpPost]
    public async Task<ActionResult<SystemParameterTypeDto>> Create(SystemParameterTypeDto dto, CancellationToken ct)
    {
        var created = await _service.Create(dto, ct);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, SystemParameterTypeDto dto, CancellationToken ct)
    {
        if (id != dto.Id) return BadRequest("Mismatched route ID and payload ID.");
        await _service.Update(dto, ct);
        return NoContent();
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(SystemParameterTypeDto dto, CancellationToken ct)
    {
        await _service.Delete(dto, ct);
        return NoContent();
    }
}
