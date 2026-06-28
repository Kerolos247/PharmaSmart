using System.ComponentModel.DataAnnotations;

namespace WebApplication4.Application.Category_Component.Category
{
    public class RequestCreateCategory
    {
        [Required(ErrorMessage = "Category name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;
    }
}
