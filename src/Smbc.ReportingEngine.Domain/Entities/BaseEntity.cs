using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smbc.ReportingEngine.Domain.Entities;

public abstract class BaseEntity
{
    public long Id { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? LastModifiedBy { get; set; }
    public DateTime? LastModifiedAt { get; set; }
    public bool IsDeleted { get; set; }
}
