using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using Smbc.Risk.ReportingEngine.Domain.Entities;
using Smbc.Risk.ReportingEngine.Domain.Repositories;
using Smbc.Risk.ReportingEngine.Domain.Shared.DataTransferObjects;

namespace Smbc.Risk.ReportingEngine.Infrastructure.Data.EntityFramework.Repositories;

internal class ReportTemplateRepository(ApplicationDbContext dbContext) 
    : BaseRepository<ReportTemplate>(dbContext), IReportTemplateRepository
{
    public async Task<long> UploadTemplateAsync(
        string name,
        string? description,
        string outputDirectory,
        string fileNamePattern,
        Stream fileStream,
        string fileName,
        List<ReportParameterDto> parameters,
        CancellationToken cancellationToken = default)
    {
        // 1. Manage Template File Storage and Versioning
        var highestVersion = await _dbContext.ReportTemplates
            .Where(t => t.Name == name)
            .MaxAsync(t => (int?)t.Version, cancellationToken) ?? 0;

        int newVersion = highestVersion + 1;

        // Deactivate previous versions
        var existingTemplates = await _dbContext.ReportTemplates
            .Where(t => t.Name == name && t.IsActive)
            .ToListAsync(cancellationToken);

        foreach (var existing in existingTemplates)
        {
            existing.IsActive = false;
        }

        var saveFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "Templates");
        Directory.CreateDirectory(saveFolder);

        var filePath = Path.Combine(saveFolder, $"{Guid.NewGuid()}_{newVersion}_{fileName}");
        await using (var fs = new FileStream(filePath, FileMode.Create))
        {
            await fileStream.CopyToAsync(fs, cancellationToken);
        }

        // 2. Extract Named Ranges using CloseXML
        fileStream.Position = 0;
        using var workbook = new XLWorkbook(fileStream);
        var extractedNamedRanges = workbook.DefinedNames
            .Select(dn => dn.Name)
            .Distinct()
            .ToList();

        // 3. Create Entity Base Model
        var template = new ReportTemplate
        {
            Name = name,
            Description = description,
            FilePath = filePath,
            Version = newVersion,
            OutputDirectory = outputDirectory,
            FileNamePattern = fileNamePattern,
            IsActive = true,
            Parameters = parameters.Select(p => new ReportParameter
            {
                Name = p.Name,
                Type = p.Type,
                IsRequired = p.IsRequired
            }).ToList(),
            Metrics = extractedNamedRanges.Select(nr => new ReportMetric
            {
                NamedRange = nr,
                SqlQuery = "SELECT 1", // Default template query
                IsActive = true
            }).ToList()
        };

        _dbContext.ReportTemplates.Add(template);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return template.Id;
    }
}
