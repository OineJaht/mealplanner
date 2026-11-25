namespace MealPlanner.Models.ViewModels.Categories
{
    public class CategoryIndexViewModel
    {
        public List<BLL.DTO.Category> Categories { get; set; } = new();

        public Dictionary<int, int> CategoryRecipeCounts { get; set; } = new();
    }
}