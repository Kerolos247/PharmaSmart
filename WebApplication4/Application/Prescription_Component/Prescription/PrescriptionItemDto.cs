using System.ComponentModel.DataAnnotations;

namespace WebApplication4.Application.Prescription_Component.Prescription
{
    public class PrescriptionItemDto
    {
        [Required(ErrorMessage = "Medicine is required")]
        public int MedicineId { get; set; }

        [Required(ErrorMessage = "Dosage is required")]
        public string Dosage { get; set; } = string.Empty;

        [Required(ErrorMessage = "Frequency is required")]
        public string Frequency { get; set; } = string.Empty;

        [Required(ErrorMessage = "Duration is required")]
        public int Duration { get; set; }
    }
}
