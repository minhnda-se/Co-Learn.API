using System;
using System.Collections.Generic;

namespace CoLearn.Infrastructure.Models;

public partial class AuditTrail
{
    public long AuditId { get; set; }

    public string TableName { get; set; } = null!;

    public string? Pkvalue { get; set; }

    public string Action { get; set; } = null!;

    public string? OldValue { get; set; }

    public string? NewValue { get; set; }

    public int? PerformedBy { get; set; }

    public DateTime PerformedAt { get; set; }

    public virtual User? PerformedByNavigation { get; set; }
}
