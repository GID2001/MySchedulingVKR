using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MySchedulingVKR.Models;

public partial class Subject
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Teacher>? Teachers { get; set; } = new List<Teacher>();

    public virtual ICollection<OrganizationSubject>? OrganizationSubjects { get; set; } = new List<OrganizationSubject>();
}
