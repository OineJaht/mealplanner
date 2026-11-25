using MealPlanner.Models.Inventory;

namespace MealPlanner.Dto
{
    public class RecipeFinderViewModel
    {
        public List<Ingredient> PantryIngredients { get; set; } = new List<Ingredient>();
        public List<Ingredient> AllIngredients { get; set; } = new List<Ingredient>();
        public List<int> SelectedIngredientIds { get; set; } = new List<int>();
    }
}