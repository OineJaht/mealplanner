using MealPlanner.Dto;

namespace MealPlanner.Models.ViewModels.Categories
{
    public class CategoryCreateViewModel
    {
        public CategoryCreateDto Category { get; set; } = new();
    }
}