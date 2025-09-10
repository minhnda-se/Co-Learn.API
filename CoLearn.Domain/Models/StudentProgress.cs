using System;
using System.Collections.Generic;

namespace CoLearn.Infrastructure.Models;

public partial class StudentProgress
{
    public long ProgressId { get; set; }

    public int EnrollmentId { get; set; }

    public int LessonId { get; set; }

    public bool Completed { get; set; }

    public DateTime? CompletedAt { get; set; }

    public decimal? Score { get; set; }

    public string? Notes { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Enrollment Enrollment { get; set; } = null!;

    public virtual Lesson Lesson { get; set; } = null!;
}
