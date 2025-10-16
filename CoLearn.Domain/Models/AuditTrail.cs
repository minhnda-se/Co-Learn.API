using System;
using System.Collections.Generic;

namespace CoLearn.Domain.Models;

public partial class AuditTrail
{
    public long AuditId { get; set; }

    public int? UserId { get; set; }

    public string Action { get; set; } = null!;

    public string TableName { get; set; } = null!;

    public int? RecordId { get; set; }

    public string? OldValue { get; set; }

    public string? NewValue { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User? User { get; set; }
}
