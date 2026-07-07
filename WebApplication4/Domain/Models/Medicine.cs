namespace WebApplication4.Domain.Models
{
    public class Medicine
    {
        public int MedicineId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string DosageForm { get; set; }
        public string Strength { get; set; }

        public decimal Price { get; set; }

        // FK
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; }

        // Relations
        public Inventory Inventory { get; set; }
        //public ICollection<PrescriptionItem> PrescriptionItems { get; set; }
    }
}
