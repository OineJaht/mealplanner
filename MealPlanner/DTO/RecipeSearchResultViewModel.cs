
using MealPlanner.Dto;
using MealPlanner.Models.Inventory;

namespace MealPlanner.Dto
{
    public class RecipeSearchResultViewModel
    {
        public List<RecipeMatchViewModel> MatchingRecipes { get; set; } = new List<RecipeMatchViewModel>();
        public List<Ingredient> SelectedIngredients { get; set; } = new List<Ingredient>();
        public bool MatchAll { get; set; }
    }

    public class RecipeMatchViewModel
    {
        public Recipe? Recipe { get; set; }
        public int MatchPercentage { get; set; }
        public List<Ingredient> MissingIngredients { get; set; } = new List<Ingredient>();
    }
}