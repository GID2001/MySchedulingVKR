using MySchedulingVKR.Models;

namespace MySchedulingVKR.ViewModels
{
    public class StudentSubjectView
    {
        public Student Student { get; set; }

        public IEnumerable<Subject> Subjects { get; set; }

    }
}
