
using System.Collections.Generic;

namespace MealPlanner.Dto
{
    public class RecipeDetailsViewModel
    {
        public RecipeDetailDto? Recipe { get; set; }
        public List<RecipeIngredientDto> RecipeIngredients { get; set; } = new List<RecipeIngredientDto>();
    }
}