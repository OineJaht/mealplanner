using MealPlanner.Dto;

namespace MealPlanner.Models.ViewModels.Nutrition
{
    public class RecipeNutritionViewModel
    {
        public int RecipeId { get; set; }
        public string RecipeName { get; set; } = string.Empty;
        public int DefaultServings { get; set; }
        public RecipeNutritionDto Nutrition { get; set; } = new RecipeNutritionDto();
    }
}