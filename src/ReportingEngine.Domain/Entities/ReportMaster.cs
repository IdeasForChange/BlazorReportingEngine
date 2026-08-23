using Smbc.Risk.Core.Domain.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smbc.Risk.ReportingEngine.Domain.Entities;

public class ReportMaster : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ReportNamePattern { get; set; } = string.Empty;
    public string ReportDirectory { get; set; } = string.Empty;

    public ICollection<ReportParameter> Parameters { get; set; } = [];
    public ICollection<ReportTemplate> Templates { get; set; } = [];
}