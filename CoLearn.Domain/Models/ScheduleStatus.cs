using System;
using System.Collections.Generic;

namespace CoLearn.Infrastructure.Models;

public partial class ScheduleStatus
{
    public int ScheduleStatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
