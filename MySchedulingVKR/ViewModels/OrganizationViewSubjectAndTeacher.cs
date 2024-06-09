using MySchedulingVKR.Models;

namespace MySchedulingVKR.ViewModels
{
    public class OrganizationViewSubjectAndTeacher
    {
        public Organization Organization { get; set; }
        
        public IEnumerable<Organization> Organizations { get; set; } 

        public IEnumerable<Subject> Subjects { get; set; } 

        public IEnumerable<Teacher> Teachers { get; set; } 

    }
}
