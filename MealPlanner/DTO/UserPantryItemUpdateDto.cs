using System.ComponentModel.DataAnnotations;

namespace MealPlanner.Dto
{
    public class UserPantryItemUpdateDto
    {
        public int Id { get; set; }
        public int IngredientId { get; set; }
        public decimal AmountInStock { get; set; }
        public string? IngredientName { get; set; }  // Add this
        public string? IngredientCategory { get; set; }  // Add this
        public IngredientNutritionDto Nutrition { get; set; } = new IngredientNutritionDto(); // Add this
        
        [Required]
        [StringLength(50)]
        public string? Unit { get; set; }
        
        [StringLength(255)]
        public string Location { get; set; } = string.Empty;
        
        public DateTime ExpiryDate { get; set; }
    }
}