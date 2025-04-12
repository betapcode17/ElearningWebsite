using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ElearningWebsite.Models;

namespace ElearningWebsite.ViewModel
{
    public class RegisterVM
    {
        [Required(ErrorMessage = "Username cannot be blank.")]
        public string Username { get; set; } = null!;
        [Required(ErrorMessage = "Password cannot be blank.")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        public string Password { get; set; } = null!;
        [Required(ErrorMessage = "Confirm Password cannot be blank.")]
        [Compare("Password", ErrorMessage = "Password and Confirm Password do not match.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = null !;
        [Required(ErrorMessage = "FullName is required.")]
        public string FullName { get; set; } = null!;
        [Required(ErrorMessage = "Date of Birth is required.")]
        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateOnly DateOfBirth { get; set; }
        [Required(ErrorMessage = "Phone cannot be blank.")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        [StringLength(15, ErrorMessage = "Phone number cannot be longer than 15 characters.")]
        public string? PhoneNumber
        {
            get; set;
                } = null!;
        [Required(ErrorMessage = "Email cannot be blank.")]
        [EmailAddress(ErrorMessage = "Invalid Email format.")]
        public string Email { get; set; } = null!;

        public string? ImagePath { get; set; }
        [Required(ErrorMessage = "Please select your gender.")]
        public int? Gender { get; set; }
        public int? Role { get; set; }
        public string? UserId { get; set; } = null!;
    }
}
