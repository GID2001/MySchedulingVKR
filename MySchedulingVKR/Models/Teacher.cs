using System;
using System.Collections.Generic;

namespace MySchedulingVKR.Models;

public partial class Teacher
{
    public int Id { get; set; }

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public int? SubjectId { get; set; }

    public int? OrganizationId { get; set; }

    public virtual Organization? Organization { get; set; }

    public virtual Subject? Subject { get; set; }

    public virtual ICollection<TeacherAccess> TeacherAccesses { get; set; } = new List<TeacherAccess>();

    public virtual ICollection<ScheduleTeacher> ScheduleTeachers { get; set; } = new List<ScheduleTeacher>();
}
