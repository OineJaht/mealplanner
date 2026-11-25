using MealPlanner.BLL.DTO;

namespace MealPlanner.Models.ViewModels
{
    public class RecipeMatchViewModel
    {
        public Recipe? Recipe { get; set; }
        public int MatchPercentage { get; set; }
        public List<Ingredient> MissingIngredients { get; set; } = new List<Ingredient>();
    }
}