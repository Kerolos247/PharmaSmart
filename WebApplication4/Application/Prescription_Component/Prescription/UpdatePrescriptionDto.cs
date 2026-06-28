using System.ComponentModel.DataAnnotations;
using WebApplication4.Application.Common.Validation;
namespace WebApplication4.Application.Prescription_Component.Prescription
{
    public class UpdatePrescriptionDto
    {
        [NotFutureDate(ErrorMessage = "Date cannot be in the future")]
        public DateTime? Date { get; set; }


        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        public string? Notes { get; set; }


        [Range(1, int.MaxValue, ErrorMessage = "Patient is required")]
        public int? PatientId { get; set; }


        [StringLength(450, ErrorMessage = "Invalid pharmacist id")]
        public string? PharmacistId { get; set; }
    }
}
