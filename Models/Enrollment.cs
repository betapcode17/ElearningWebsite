using System;
using System.Collections.Generic;

namespace ElearningWebsite.Models;

public partial class Enrollment
{
    public string EnrollmentId { get; set; } = null!;

    public string StudentId { get; set; } = null!;

    public string CourseId { get; set; } = null!;

    public DateOnly EnrollmentDate { get; set; }

    public string Status { get; set; } = null!;

    public virtual Course Course { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
