using System.ComponentModel.DataAnnotations;

namespace MealPlanner.Dto
{
    public class IngredientUpdateDto
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(255)]
        public string? Name { get; set; }
        
        [StringLength(255)]
        public string Category { get; set; } = string.Empty;
    }
}