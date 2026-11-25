namespace MealPlanner.Models.ViewModels.Categories
{
    public class CategoryDeleteViewModel
    {
        public BLL.DTO.Category Category { get; set; } = new();

        public int RecipeCount { get; set; }
    }
}