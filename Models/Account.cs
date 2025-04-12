using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace ElearningWebsite.Models;

public partial class Account
{
    public string? UserId { get; set; } = null!;
    public string Username { get; set; } = null!;  
    public string Password { get; set; } = null!;
    public int? Role { get; set; }
    public virtual Admin? Admin { get; set; }

    public virtual Student? Student { get; set; }
}
