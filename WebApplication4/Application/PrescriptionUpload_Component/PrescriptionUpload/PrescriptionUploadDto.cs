using System.ComponentModel.DataAnnotations;
using WebApplication4.Application.Common.Validation;
namespace WebApplication4.Application.PrescriptionUpload_Component.PrescriptionUpload
{
    public class PrescriptionUploadDto
    {
        [Required(ErrorMessage = "Full name is required.")]
        [MaxLength(50, ErrorMessage = "Full name cannot exceed 50 characters.")]
        [MinLength(3, ErrorMessage = "Full name must be at least 3 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Full name can only contain letters and spaces.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^(?:\+20|0)?1[0125][0-9]{8}$", ErrorMessage = "Invalid Egyptian phone number. Must start with 010, 011, 012, 015 and contain 11 digits")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required.")]
        [MaxLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        [MinLength(5, ErrorMessage = "Address must be at least 5 characters.")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Prescription file is required.")]
        [DataType(DataType.Upload)]
        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png" })]
        [MaxFileSize(5 * 1024 * 1024)] // 5MB
        public IFormFile File { get; set; } = default!;

    }
}
