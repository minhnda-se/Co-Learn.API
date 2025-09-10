using System;
using System.Collections.Generic;

namespace CoLearn.Infrastructure.Models;

public partial class Lesson
{
    public int LessonId { get; set; }

    public int CourseId { get; set; }

    public string Title { get; set; } = null!;

    public string? Content { get; set; }

    public byte? OrderNumber { get; set; }

    public short? DurationMinutes { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();

    public virtual Course Course { get; set; } = null!;

    public virtual ICollection<CourseMaterial> CourseMaterials { get; set; } = new List<CourseMaterial>();

    public virtual ICollection<StudentProgress> StudentProgresses { get; set; } = new List<StudentProgress>();
}
