using Microsoft.AspNetCore.Mvc.Rendering;
using MealPlanner.Dto;

namespace MealPlanner.Models.Inventory.ViewModels.Recipes
{
    public class RecipeEditViewModel
    {
        public RecipeUpdateDto Recipe { get; set; } = new RecipeUpdateDto();
        
        // Available options for dropdowns that were previously in ViewData
        public SelectList AvailableCategories { get; set; } = null!;
        public SelectList AvailableIngredients { get; set; } = null!;
        
        // Selected values that were previously in ViewData
        public List<int> SelectedCategoryIds { get; set; } = new List<int>();
        
        // Dictionary of ingredient ID to amount and unit data
        public Dictionary<int, RecipeIngredientInfo> SelectedIngredients { get; set; } = 
            new Dictionary<int, RecipeIngredientInfo>();
    }

    // Helper class to store ingredient-related information
    public class RecipeIngredientInfo
    {
        public decimal Amount { get; set; }
        public string Unit { get; set; } = string.Empty;
    }
}