using System;
using System.Collections.Generic;

namespace CoLearn.Infrastructure.Models;

public partial class CourseMaterial
{
    public int MaterialId { get; set; }

    public int? LessonId { get; set; }

    public int? CourseId { get; set; }

    public string? Title { get; set; }

    public string? MaterialType { get; set; }

    public string? Url { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Course? Course { get; set; }

    public virtual Lesson? Lesson { get; set; }
}
