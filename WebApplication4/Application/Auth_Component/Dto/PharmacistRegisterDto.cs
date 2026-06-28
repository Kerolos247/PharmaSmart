using System.ComponentModel.DataAnnotations;

namespace WebApplication4.Application.Auth_Component.Dto
{
    public class PharmacistRegisterDto
    {


        [Required(ErrorMessage = "Full name is required")]
        [StringLength(150, MinimumLength = 5,
            ErrorMessage = "Full name must be at least 5 characters")]
        [RegularExpression(@"^[a-zA-Z\u0600-\u06FF]+(\s+[a-zA-Z\u0600-\u06FF]+)+$",
            ErrorMessage = "Full name must contain at least first and last name")]
        public string FullName { get; set; } = string.Empty;


        [Required(ErrorMessage = "Email is required")]
        [StringLength(150, ErrorMessage = "Email cannot exceed 150 characters")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;


        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 8,
            ErrorMessage = "Password must be at least 8 characters long")]
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]+$",
            ErrorMessage = "Password must contain uppercase, lowercase, number and special character")]
        public string Password { get; set; } = string.Empty;
    }
}
