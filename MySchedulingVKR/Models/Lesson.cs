using System;
using System.Collections.Generic;

namespace MySchedulingVKR.Models;

public partial class Lesson
{
    public int Id { get; set; }

    public string DayOfTheWeek { get; set; } = null!;

    public string Time { get; set; } = null!;

    public DateOnly Date { get; set; }

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();

    public virtual ICollection<TeacherAccess> TeacherAccesses { get; set; } = new List<TeacherAccess>();

    public enum DayOfWeekEnum
    {
        Понедельник,
        Вторник,
        Среда,
        Четверг,
        Пятница,
        Суббота,
        Воскресенье
    }

}
