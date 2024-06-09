using System;
using System.Collections.Generic;

namespace MySchedulingVKR.Models;

public partial class ScheduleStudent
{
    public int ScheduleId { get; set; }

    public int StudentId { get; set; }

    public virtual Schedule Schedule { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
