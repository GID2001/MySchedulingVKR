using System;
using System.Collections.Generic;

namespace MySchedulingVKR.Models;

public partial class OrganizationSubject
{
    public int OrganizationId { get; set; }

    public int SubjectId { get; set; }

    public virtual Organization? Organization { get; set; } = null!;

    public virtual Subject? Subject { get; set; } = null!;
}
