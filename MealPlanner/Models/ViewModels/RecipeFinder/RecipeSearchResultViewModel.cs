using MealPlanner.BLL.DTO;
using MealPlanner.Dto;

namespace MealPlanner.Models.ViewModels
{
    public class RecipeSearchResultViewModel
    {
        public List<RecipeMatchViewModel> MatchingRecipes { get; set; } = new List<RecipeMatchViewModel>();
        public List<Ingredient> SelectedIngredients { get; set; } = new List<Ingredient>();
        public bool MatchAll { get; set; }
    }
}