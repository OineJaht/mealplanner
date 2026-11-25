using MealPlanner.Dto;

namespace MealPlanner.Models.ViewModels.Ingredients
{
    public class IngredientEditViewModel
    {
        public IngredientUpdateDto Ingredient { get; set; } = new IngredientUpdateDto();
        
        // Nutrition data that was previously stored in ViewBag
        public IngredientNutritionUpdateDto Nutrition { get; set; } = new IngredientNutritionUpdateDto();
    }
}