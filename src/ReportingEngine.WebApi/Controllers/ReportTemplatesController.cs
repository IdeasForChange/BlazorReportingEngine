using ClosedXML.Excel;
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
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default) => Ok(await _service.GetAll(cancellationToken));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken = default)
    {
        var result = await _service.GetById(id, cancellationToken);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ReportTemplateDto dto, CancellationToken cancellationToken = default)
    {
        var result = await _service.Create(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] ReportTemplateDto dto, CancellationToken cancellationToken = default)
    {
        if (id != dto.Id) return BadRequest();
        await _service.Update(dto, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id, [FromBody] ReportTemplateDto dto, CancellationToken cancellationToken = default)
    {
        if (id != dto.Id) return BadRequest();
        await _service.Delete(dto, cancellationToken);
        return NoContent();
    }

    [HttpPost("extract-named-ranges")]
    public async Task<IActionResult> ExtractNamedRanges(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (extension != ".xlsx" && extension != ".xls")
            return BadRequest("Only Excel files (.xlsx, .xls) are allowed.");

        var namedRanges = new List<string>();

        using (var stream = new MemoryStream())
        {
            await file.CopyToAsync(stream);
            stream.Position = 0;

            using (var workbook = new XLWorkbook(stream))
            {
                // Extract workbook-scoped named ranges
                foreach (var range in workbook.NamedRanges)
                {
                    namedRanges.Add(range.Name);
                }

                // Extract worksheet-scoped named ranges
                foreach (var sheet in workbook.Worksheets)
                {
                    foreach (var range in sheet.NamedRanges)
                    {
                        var fullName = $"{sheet.Name}!{range.Name}";
                        if (!namedRanges.Contains(fullName))
                        {
                            namedRanges.Add(fullName);
                        }
                    }
                }
            }
        }

        return Ok(new { FileName = file.FileName, NamedRanges = namedRanges });
    }
}
