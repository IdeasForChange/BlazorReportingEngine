using Microsoft.AspNetCore.Mvc;
using Smbc.Risk.ReportingEngine.Domain.Services;
using Smbc.Risk.ReportingEngine.Domain.Shared.DataTransferObjects;

namespace Smbc.Risk.ReportingEngine.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportTemplatesController(IReportTemplateService service) : ControllerBase
{
    private readonly IReportTemplateService _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default) 
    {
        var results = await _service.GetAllReportsAsync(cancellationToken);
        return Ok(results);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken = default)
    {
        var report = await _service.GetReportByIdAsync(id, cancellationToken);
        return report == null ? NotFound() : Ok(report);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateReportTemplateRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _service.CreateReportWithTemplateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("metrics/{id}")]
    public async Task<IActionResult> UpdateMetric(long id, [FromBody] ReportMetricDto metricDto, CancellationToken cancellationToken = default)
    {
        metricDto.Id = id;
        await _service.UpdateMetricAsync(metricDto, cancellationToken);
        return NoContent();
    }

    [HttpPost("parameters")]
    public async Task<IActionResult> AddParameter([FromBody] ReportParameterDto parameterDto, CancellationToken cancellationToken = default)
    {
        await _service.AddParameterAsync(parameterDto, cancellationToken);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken = default)
    {
        await _service.DeleteReportAsync(id, cancellationToken);
        return NoContent();
    }
}