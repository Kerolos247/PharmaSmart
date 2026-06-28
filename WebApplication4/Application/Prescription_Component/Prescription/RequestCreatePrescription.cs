using System.ComponentModel.DataAnnotations;
using WebApplication4.Application.Common.Validation;
namespace WebApplication4.Application.Prescription_Component.Prescription
{
    public class RequestCreatePrescription
    {
        [Required(ErrorMessage = "Date is required")]
        [NotFutureDate(ErrorMessage = "Date cannot be in the future")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Notes are required")]
        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        public string Notes { get; set; } = string.Empty;

        [Required(ErrorMessage = "Patient is required")]
        public int PatientId { get; set; }

        [Required(ErrorMessage = "Pharmacist is required")]
        public string PharmacistId { get; set; } = string.Empty;
    }
}
