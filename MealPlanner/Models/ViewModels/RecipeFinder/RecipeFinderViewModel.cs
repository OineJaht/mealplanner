using MealPlanner.BLL.DTO;

namespace MealPlanner.Models.ViewModels;

public class RecipeFinderViewModel
{
    public List<Ingredient> PantryIngredients { get; set; } = new();
    public List<Ingredient> AllIngredients { get; set; } = new();
    public List<int> SelectedIngredientIds { get; set; } = new();
}