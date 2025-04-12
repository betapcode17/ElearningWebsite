using System;
using System.Collections.Generic;

namespace ElearningWebsite.Models;

public partial class Course
{
    public string CourseId { get; set; } = null!;

    public string CourseName { get; set; } = null!;

    public string Instructor { get; set; } = null!;

    public DateOnly StartDate { get; set; }

    public decimal TuitionFee { get; set; }

    public int MaxStudents { get; set; }

    public int VideoCount { get; set; }

    public int? CurrentStudents { get; set; }

    public string? ImgPath { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
