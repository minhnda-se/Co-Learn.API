using System;
using System.Collections.Generic;

namespace CoLearn.Domain.Models;

public partial class SystemLog
{
    public long LogId { get; set; }

    public int? ErrorTypeId { get; set; }

    public int? UserId { get; set; }

    public string? Action { get; set; }

    public string? Message { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ErrorType? ErrorType { get; set; }

    public virtual User? User { get; set; }
}
