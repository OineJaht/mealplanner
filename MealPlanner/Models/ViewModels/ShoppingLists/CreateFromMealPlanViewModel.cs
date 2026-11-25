using Microsoft.AspNetCore.Mvc.Rendering;

namespace MealPlanner.Models.ViewModels.ShoppingLists
{
    public class CreateFromMealPlanViewModel
    {
        public int MealPlanId { get; set; }
        public string ListName { get; set; } = string.Empty;
        
        // Dropdown data that was previously in ViewData
        public SelectList AvailableMealPlans { get; set; } = null!;
    }
}