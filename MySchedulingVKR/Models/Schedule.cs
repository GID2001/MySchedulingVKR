using System;
using System.Collections.Generic;

namespace MySchedulingVKR.Models;

public partial class Schedule
{
    public int LessonId { get; set; }

    public virtual Lesson Lesson { get; set; } = null!;

    public virtual ICollection<ScheduleTeacher> ScheduleTeachers { get; set; } = new List<ScheduleTeacher>();

    public virtual ICollection<ScheduleStudent> ScheduleStudents  { get; set; } = new List<ScheduleStudent>();
}
