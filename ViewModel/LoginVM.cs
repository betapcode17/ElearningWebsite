using System.ComponentModel.DataAnnotations;

namespace ElearningWebsite.ViewModel
{
    public class LoginVM
    {
        [Required(ErrorMessage = "Username cannot be blank.")]
        public string Username { get; set; } = null!;
        [Required(ErrorMessage = "Password cannot be blank.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

    }
}
