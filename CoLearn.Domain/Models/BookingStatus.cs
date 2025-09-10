using System;
using System.Collections.Generic;

namespace CoLearn.Infrastructure.Models;

public partial class BookingStatus
{
    public int StatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
