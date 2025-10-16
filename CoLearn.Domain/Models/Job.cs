using System;
using System.Collections.Generic;

namespace CoLearn.Domain.Models;

public partial class Job
{
    public int JobId { get; set; }

    public string JobType { get; set; } = null!;

    public string? HangfireJobId { get; set; }

    public string Status { get; set; } = null!;

    public DateTime RunAt { get; set; }

    public DateTime? ExecutedAt { get; set; }

    public string? ErrorMessage { get; set; }

    public int RetryCount { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<JobTarget> JobTargets { get; set; } = new List<JobTarget>();
}
