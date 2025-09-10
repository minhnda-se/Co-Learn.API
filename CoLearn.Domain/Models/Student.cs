using System;
using System.Collections.Generic;

namespace CoLearn.Infrastructure.Models;

public partial class Student
{
    public int StudentId { get; set; }

    public int UserId { get; set; }

    public int? ParentId { get; set; }

    public string? GradeLevel { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    public virtual Parent? Parent { get; set; }

    public virtual ICollection<Submission> Submissions { get; set; } = new List<Submission>();

    public virtual User User { get; set; } = null!;
}
