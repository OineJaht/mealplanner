namespace MealPlanner.Models.ViewModels.Categories
{
    public class CategoryDetailsViewModel
    {
        public BLL.DTO.Category Category { get; set; } = new();

        public int RecipeCount { get; set; }

        public List<BLL.DTO.Recipe> AssociatedRecipes { get; set; } = new();
    }
}