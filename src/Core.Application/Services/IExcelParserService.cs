using ClosedXML.Excel;

namespace Smbc.Risk.Core.Application.Services;

public interface IExcelParserService
{
    List<string> ExtractNamedRanges(Stream fileStream);
    List<string> ExtractNamedRanges(IXLWorkbook workbook);
}
