using System;
using System.Collections.Generic;

namespace CoLearn.Infrastructure.Models;

public partial class CourseCategory
{
    public int CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
}
