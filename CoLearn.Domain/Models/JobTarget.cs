using System;
using System.Collections.Generic;

namespace CoLearn.Domain.Models;

public partial class JobTarget
{
    public int JobTargetId { get; set; }

    public int JobId { get; set; }

    public string TargetType { get; set; } = null!;

    public int TargetId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Job Job { get; set; } = null!;
}
