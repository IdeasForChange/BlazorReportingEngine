using ClosedXML.Excel;
using Smbc.Risk.Core.Application.Services;

namespace Smbc.Risk.ReportingEngine.Application.Services;

public class ExcelParserService : IExcelParserService
{
    public List<string> ExtractNamedRanges(Stream fileStream)
    {
        var result = new List<string>();

        using var workbook = new XLWorkbook(fileStream);
        if (workbook != null && workbook.DefinedNames != null)
        {
            result = workbook.Worksheets
                .SelectMany(ws => ws.DefinedNames)
                .Select(nr => nr.Name)
                .Distinct()
                .ToList();

            //foreach (var wsNamedRange in workbook.Worksheets.SelectMany(ws => ws.DefinedNames))
            //{
            //    Console.WriteLine($"{wsNamedRange.Name} = {wsNamedRange.RefersTo}");
            //}
        }


        return result;
    }
}
