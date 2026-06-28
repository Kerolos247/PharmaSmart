using System.ComponentModel.DataAnnotations;

namespace WebApplication4.Application.Supplier_Component.Supplier
{
    public class RequestCreateSupplier
    {
        [Required(ErrorMessage = "Supplier name is required")]
        [StringLength(100, MinimumLength = 3,
         ErrorMessage = "Supplier name must be between 3 and 100 characters")]
        [RegularExpression(@"^[a-zA-Z\u0600-\u06FF\s]+$",
         ErrorMessage = "Supplier name must contain letters only")]
        public string Name { get; set; } = string.Empty;


        [RegularExpression(@"^(01)[0-9]{9}$",
            ErrorMessage = "Phone number must be a valid Egyptian mobile number")]
        public string Phone { get; set; }


        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
    }
}
