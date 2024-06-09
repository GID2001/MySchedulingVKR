using System;
using System.Collections.Generic;

namespace MySchedulingVKR.Models;

public partial class ScheduleTeacher
{
    public int ScheduleId { get; set; }

    public int TeacherId { get; set; }

    public virtual Schedule? Schedule { get; set; } = null!;

    public virtual Teacher? Teacher { get; set; } = null!;
}
