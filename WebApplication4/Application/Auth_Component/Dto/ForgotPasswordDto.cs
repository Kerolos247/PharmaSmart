using System.ComponentModel.DataAnnotations;

namespace WebApplication4.Application.Auth_Component.Dto
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
