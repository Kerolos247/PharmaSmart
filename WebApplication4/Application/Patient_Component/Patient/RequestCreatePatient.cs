using System.ComponentModel.DataAnnotations;

namespace WebApplication4.Application.Patient_Component.Patient
{
    public class RequestCreatePatient
    {
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, MinimumLength = 3,
         ErrorMessage = "Full name must be between 3 and 100 characters")]
        [RegularExpression(@"^[a-zA-Z\u0600-\u06FF\s]+$",
         ErrorMessage = "Full name must contain letters only")]
        public string FullName { get; set; } = string.Empty;


        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^(01)[0-9]{9}$",
            ErrorMessage = "Phone number must be a valid Egyptian mobile number")]
        public string Phone { get; set; } = string.Empty;


        [Required(ErrorMessage = "Address is required")]
        [StringLength(200, MinimumLength = 5,
            ErrorMessage = "Address must be at least 5 characters long")]
        public string Address { get; set; } = string.Empty;
    }
}
