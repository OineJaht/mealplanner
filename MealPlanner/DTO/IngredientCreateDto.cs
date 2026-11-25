using System.ComponentModel.DataAnnotations;

namespace MealPlanner.Dto
{
    public class IngredientCreateDto
    {
        [Required]
        [StringLength(255)]
        public string? Name { get; set; }
        
        [StringLength(255)]
        public string Category { get; set; } = string.Empty;
        
        // Optional nutrition information
        public IngredientNutritionCreateDto Nutrition { get; set; } = new IngredientNutritionCreateDto();
    }
}