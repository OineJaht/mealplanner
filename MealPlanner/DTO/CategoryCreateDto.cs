using System.ComponentModel.DataAnnotations;

namespace MealPlanner.Dto
{
    public class CategoryCreateDto
    {
        [Required]
        [StringLength(255)]
        public string? Name { get; set; }
        
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
    }
}