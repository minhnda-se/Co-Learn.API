using System;
using System.Collections.Generic;

namespace CoLearn.Infrastructure.Models;

public partial class Teacher
{
    public int TeacherId { get; set; }

    public int UserId { get; set; }

    public string? Bio { get; set; }

    public string? Qualification { get; set; }

    public byte? YearsExperience { get; set; }

    public string? VerificationStatus { get; set; }

    public decimal? HourlyRate { get; set; }

    public decimal? AvgRating { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();

    public virtual User User { get; set; } = null!;
}
