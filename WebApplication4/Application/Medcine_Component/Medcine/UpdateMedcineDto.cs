using System.ComponentModel.DataAnnotations;

namespace WebApplication4.Application.Medcine_Component.Medcine
{
    public class UpdateMedcineDto
    {
        [StringLength(100, MinimumLength = 2,
         ErrorMessage = "Medicine name must be between 2 and 100 characters")]
        public string? Name { get; set; }

        [StringLength(500, MinimumLength = 5,
            ErrorMessage = "Description must be between 5 and 500 characters")]
        public string? Description { get; set; }

        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "Dosage form must be between 2 and 100 characters")]
        public string? DosageForm { get; set; }

        [StringLength(100, MinimumLength = 1,
            ErrorMessage = "Strength must not exceed 100 characters")]
        public string? Strength { get; set; }

        // ===== Price =====

        [Range(0.01, double.MaxValue,
            ErrorMessage = "Price must be greater than zero")]
        public decimal? Price { get; set; }

        // ===== Relations =====

        [Range(1, int.MaxValue,
            ErrorMessage = "CategoryId must be a valid value")]
        public int? CategoryId { get; set; }

        [Range(1, int.MaxValue,
            ErrorMessage = "SupplierId must be a valid value")]
        public int? SupplierId { get; set; }

        // ===== Inventory =====

        [Range(0, int.MaxValue,
            ErrorMessage = "Quantity cannot be negative")]
        public int? Quantity { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ExpiryDate { get; set; }
    }
}
