using System;
using System.Collections.Generic;

namespace MySchedulingVKR.Models;

public partial class Organization
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Specialization {  get; set; } = null!;

    public virtual ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();

    public virtual ICollection<OrganizationSubject> OrganizationSubjects { get; set; } = new List<OrganizationSubject>();

}
