namespace WebApplication4.Application.Prescription_Component.Prescription
{
    public class UpdatePrescriptionItemDto
    {
        public int? PrescriptionItemId { get; set; }

        public int? MedicineId { get; set; }

        public string? Dosage { get; set; }

        public string? Frequency { get; set; }

        public int? Duration { get; set; }
    }
}
