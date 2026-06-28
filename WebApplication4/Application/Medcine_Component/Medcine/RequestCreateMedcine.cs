using System.ComponentModel.DataAnnotations;
using WebApplication4.Application.Common.Validation;
namespace WebApplication4.Application.Medcine_Component.Medcine
{
    public class RequestCreateMedcine
    {
        [Required(ErrorMessage = "Medicine name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]


        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Dosage form is required")]
        [StringLength(100, ErrorMessage = "Dosage form cannot exceed 100 characters")]



        public string DosageForm { get; set; } = string.Empty;

        [Required(ErrorMessage = "Strength is required")]
        [StringLength(100, ErrorMessage = "Strength cannot exceed 100 characters")]
        public string Strength { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "CategoryId is required")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "SupplierId is required")]
        public int SupplierId { get; set; }


        [Required(ErrorMessage = "Quantity is required")]
        [Range(10, int.MaxValue, ErrorMessage = "Quantity must be at least 10")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Expiry date is required")]
        [DataType(DataType.Date)]

        [CheckExpyired]
        public DateTime ExpiryDate { get; set; }
    }

}
