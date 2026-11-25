using MealPlanner.Dto;

namespace MealPlanner.Models.ViewModels.Categories
{
    public class CategoryEditViewModel
    {
        public CategoryUpdateDto Category { get; set; } = new();

        // Optional for warning message
        public int RecipeCount { get; set; }
    }
}