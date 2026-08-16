using Smbc.Risk.ReportingEngine.Domain.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smbc.Risk.ReportingEngine.Domain.Shared.DataTransferObjects;

public record EnqueueReportRequestDto(
    long ReportTemplateId,
    Dictionary<string, string> ParameterValues
);

public record ReportQueueDto(
    long Id,
    long ReportTemplateId,
    string ReportName,
    QueueStatus Status,
    int ProgressPercentage,
    string? OutputFilePath,
    string? ErrorMessage,
    DateTime CreatedAtUtc
);