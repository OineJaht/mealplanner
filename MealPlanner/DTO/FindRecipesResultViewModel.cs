using MealPlanner.BLL.DTO;

namespace MealPlanner.Dto;

public class FindRecipesResultViewModel
{
    public List<RecipeMatch> Matches { get; set; } = new();
    public List<Ingredient> SelectedIngredients { get; set; } = new();
    public bool MatchAll { get; set; }
}
