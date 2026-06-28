namespace WebApplication4.Application.Inventory_Component.Dto
{
    public class InventoryDto
    {
        public int InventoryId { get; set; }
        public int Quantity { get; set; }
        public DateTime ExpiryDate { get; set; }


        public int MedicineId { get; set; }
        public string MedicineName { get; set; } = string.Empty;
        public string DosageForm { get; set; } = string.Empty;
        public string Strength { get; set; } = string.Empty;
    }
}
