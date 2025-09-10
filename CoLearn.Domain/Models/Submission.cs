using System;
using System.Collections.Generic;

namespace CoLearn.Infrastructure.Models;

public partial class Submission
{
    public long SubmissionId { get; set; }

    public int AssignmentId { get; set; }

    public int StudentId { get; set; }

    public DateTime SubmittedAt { get; set; }

    public decimal? Grade { get; set; }

    public string? Feedback { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Assignment Assignment { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
