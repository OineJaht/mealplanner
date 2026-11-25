using MealPlanner.Dto;

namespace MealPlanner.Models.ViewModels.CookingSteps
{
    public class CookingStepDeleteViewModel
    {
        public CookingStepDto CookingStep { get; set; } = new CookingStepDto();
        
        // Data that was previously in ViewData
        public int RecipeId { get; set; }
        public string RecipeName { get; set; } = string.Empty;
    }
}