using System.ComponentModel.DataAnnotations;

namespace WebApplication4.Application.Modules.Auth.Dto
{
    public class ResetPasswordDto
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }

        public string Token { get; set; }
    }
}
