using MealPlanner.Dto;

namespace MealPlanner.Models.ViewModels.CookingSteps
{
    public class CookingStepEditViewModel
    {
        public CookingStepUpdateDto CookingStep { get; set; } = new CookingStepUpdateDto();
        
        // Data that was previously in ViewData
        public int RecipeId { get; set; }
        public string RecipeName { get; set; } = string.Empty;
    }
}