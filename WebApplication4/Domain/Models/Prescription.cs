using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication4.Domain.Enums;
namespace WebApplication4.Domain.Models
{
   
    public class Prescription
    {
        public int PrescriptionId { get; set; }
        public DateTime Date { get; set; }
        public string Notes { get; set; }

        // FK Patient
        [ForeignKey("Patient")]
        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        // FK Pharmacist (IdentityUser)
        [ForeignKey("Pharmacist")]
        public string PharmacistId { get; set; }
        public Pharmacist Pharmacist { get; set; }

        //// Items
        //public ICollection<PrescriptionItem> PrescriptionItems { get; set; }

        public PrescriptionStatus Status { get; set; } = PrescriptionStatus.Unpaid;
        [Timestamp]
        public byte[]? RowVersion { get; set; }

    }
}
