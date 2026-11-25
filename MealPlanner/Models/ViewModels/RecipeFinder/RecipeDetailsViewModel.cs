using System.Collections.Generic;
using MealPlanner.BLL.DTO;

namespace MealPlanner.Models.ViewModels
{
    public class RecipeDetailsViewModel
    {
        public Recipe? Recipe { get; set; }
        public List<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();
    }
}