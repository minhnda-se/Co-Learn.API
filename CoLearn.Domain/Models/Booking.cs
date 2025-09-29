using System;
using System.Collections.Generic;

namespace CoLearn.Domain.Models;

public partial class Booking
{
    public int BookingId { get; set; }

    public int TeacherId { get; set; }

    public int StudentId { get; set; }

    public int BookingStatusId { get; set; }

    public int? ScheduleId { get; set; }

    public DateTime? RequestedStartTime { get; set; }

    public DateTime? RequestedEndTime { get; set; }

    public string? Notes { get; set; }

    public bool IsPaid { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual BookingStatus BookingStatus { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Schedule? Schedule { get; set; }

    public virtual Student Student { get; set; } = null!;

    public virtual Teacher Teacher { get; set; } = null!;
}
