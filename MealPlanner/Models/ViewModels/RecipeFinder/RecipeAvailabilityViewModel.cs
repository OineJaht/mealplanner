using System.Collections.Generic;
using MealPlanner.Dto;

namespace MealPlanner.Models.ViewModels
{
    public class RecipeAvailabilityViewModel
    {
        public RecipeDetailDto Recipe { get; set; } = null!;
        
        // Available and missing ingredients information
        public List<RecipeIngredientDto> RecipeIngredients { get; set; } = new List<RecipeIngredientDto>();
        public List<int> AvailableIngredientIds { get; set; } = new List<int>();
    }
}