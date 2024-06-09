using MySchedulingVKR.Models;

namespace MySchedulingVKR.ViewModels
{
    public class StudentTeacherView
    {
        public Student Student { get; set; }

        public IEnumerable<Student> Students { get; set; }

        public Teacher Teacher { get; set; }

        public IEnumerable<Teacher> Teachers { get; set; }

        public IEnumerable<Lesson> Lessons { get; set; }

    }
}
