using MealPlanner.Dto;

namespace MealPlanner.Models.ViewModels.Nutrition
{
    public class NutritionSummaryViewModel
    {
        public List<MealPlanDetailDto> MealPlans { get; set; } = new List<MealPlanDetailDto>();

        // You could add additional properties here like:
        public decimal TotalCalories => MealPlans
            .SelectMany(mp => mp.MealPlanItems)
            .Sum(item => item.Recipe?.Nutrition?.CaloriesPerServing * item.Servings ?? 0);

        public decimal TotalProtein => MealPlans
            .SelectMany(mp => mp.MealPlanItems)
            .Sum(item =>
                item.Recipe?.Nutrition?.TotalProteinG * item.Servings /
                (item.Recipe?.DefaultServings > 0 ? item.Recipe.DefaultServings : 1) ?? 0);

        // Add similar properties for other nutrients
    }
}