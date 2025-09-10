using System;
using System.Collections.Generic;

namespace CoLearn.Infrastructure.Models;

public partial class Assignment
{
    public int AssignmentId { get; set; }

    public int LessonId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? DueDate { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Lesson Lesson { get; set; } = null!;

    public virtual ICollection<Submission> Submissions { get; set; } = new List<Submission>();
}
