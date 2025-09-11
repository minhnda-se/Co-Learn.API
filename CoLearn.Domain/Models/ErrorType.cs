using System;
using System.Collections.Generic;

namespace CoLearn.Domain.Models;

public partial class ErrorType
{
    public int ErrorTypeId { get; set; }

    public string ErrorName { get; set; } = null!;

    public virtual ICollection<SystemLog> SystemLogs { get; set; } = new List<SystemLog>();
}
