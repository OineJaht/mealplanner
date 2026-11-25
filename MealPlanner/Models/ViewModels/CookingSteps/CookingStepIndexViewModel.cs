using System.Collections.Generic;
using MealPlanner.Dto;

namespace MealPlanner.Models.ViewModels.CookingSteps
{
    public class CookingStepIndexViewModel
    {
        public List<CookingStepDto> CookingSteps { get; set; } = new List<CookingStepDto>();
        
        // Data that was previously in ViewData
        public int RecipeId { get; set; }
        public string RecipeName { get; set; } = string.Empty;
    }
}