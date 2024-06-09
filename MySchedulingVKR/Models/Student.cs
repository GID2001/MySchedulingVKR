using System;
using System.Collections.Generic;

namespace MySchedulingVKR.Models;

public partial class Student
{
    public int Id { get; set; }

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public virtual ICollection<ScheduleStudent> ScheduleStudents { get; set; } = new List<ScheduleStudent>();
}
