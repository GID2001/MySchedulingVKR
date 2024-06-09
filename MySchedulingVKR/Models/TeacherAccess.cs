using System;
using System.Collections.Generic;

namespace MySchedulingVKR.Models;

public partial class TeacherAccess
{
    public int TeacherId { get; set; }
    public int LessonId { get; set; }
    public bool Access { get; set; }

    public virtual Teacher? Teacher { get; set; } = null!;
    public virtual Lesson? Lesson { get; set; } = null!;
     
}
