using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smbc.Risk.Core.Application.Services;

public interface IExcelParserService
{
    List<string> ExtractNamedRanges(Stream fileStream);
}
