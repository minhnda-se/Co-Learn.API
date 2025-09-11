using System;
using System.Collections.Generic;

namespace CoLearn.Domain.Models;

public partial class Course
{
    public int CourseId { get; set; }

    public int? TeacherId { get; set; }

    public int? CategoryId { get; set; }

    public string Title { get; set; } = null!;

    public string? ShortDescription { get; set; }

    public string? Description { get; set; }

    public string? Level { get; set; }

    public decimal? PricePerSession { get; set; }

    public short? DurationMinutes { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual CourseCategory? Category { get; set; }

    public virtual ICollection<CourseMaterial> CourseMaterials { get; set; } = new List<CourseMaterial>();

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();

    public virtual Teacher? Teacher { get; set; }
}
