using System;
using System.Collections.Generic;

namespace CoLearn.Domain.Models;

public partial class Schedule
{
    public int ScheduleId { get; set; }

    public int? CourseId { get; set; }

    public int TeacherId { get; set; }

    public int? StudentId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public string? MeetingLink { get; set; }

    public byte MaxStudents { get; set; }

    public byte CurrentStudents { get; set; }

    public int ScheduleStatusId { get; set; }

    public bool IsRecurring { get; set; }

    public string? RecurrenceRule { get; set; }

    public bool IsCompleted { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Course? Course { get; set; }

    public virtual ScheduleStatus ScheduleStatus { get; set; } = null!;

    public virtual Student? Student { get; set; }

    public virtual Teacher Teacher { get; set; } = null!;
}
