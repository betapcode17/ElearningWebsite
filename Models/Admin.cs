using System;
using System.Collections.Generic;

namespace ElearningWebsite.Models;

public partial class Admin
{
    public string AdminId { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string? ImagePath { get; set; }

    public virtual Account AdminNavigation { get; set; } = null!;
}
