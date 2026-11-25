using MealPlanner.Dto;
// Models/ViewModels/Nutrition/IngredientNutritionViewModel.cs
namespace MealPlanner.Models.ViewModels.Nutrition
{
    public class IngredientNutritionViewModel
    {
        public int IngredientId { get; set; }
        public string IngredientName { get; set; } = string.Empty;
        public string IngredientCategory { get; set; } = string.Empty;
        public IngredientNutritionDto Nutrition { get; set; } = new IngredientNutritionDto();
    }
}