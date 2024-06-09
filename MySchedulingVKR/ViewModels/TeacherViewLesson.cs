using MySchedulingVKR.Models;

namespace MySchedulingVKR.ViewModels
{
    public class TeacherViewLesson
    {
        public Teacher Teacher { get; set; }

        public IEnumerable<Teacher> Teachers { get; set; }

        public IEnumerable<TeacherAccess> TeachersAccesses { get; set;}

        public IEnumerable<Lesson> Lessons { get; set; }
    }
}
