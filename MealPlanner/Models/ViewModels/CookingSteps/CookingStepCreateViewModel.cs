using MealPlanner.Dto;

namespace MealPlanner.Models.ViewModels.CookingSteps
{
    public class CookingStepCreateViewModel
    {
        public CookingStepCreateDto CookingStep { get; set; } = new CookingStepCreateDto();
        
        // Data that was previously in ViewData
        public int RecipeId { get; set; }
        public string RecipeName { get; set; } = string.Empty;
    }
}