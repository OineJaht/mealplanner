using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using MealPlanner.Dto;

namespace MealPlanner.Models.ViewModels
{
    public class RecipeCreateViewModel
    {
        [Required]
        public RecipeCreateDto Recipe { get; set; } = new RecipeCreateDto();

        // 🔧 FIX: These should NOT have [Required] attributes
        // They are UI helper properties, not data to be validated
        public SelectList? AvailableCategories { get; set; }
        public SelectList? AvailableIngredients { get; set; }

        // These are the actual data that should be validated
        public List<int> SelectedCategoryIds { get; set; } = new List<int>();
        public List<int> SelectedIngredientIds { get; set; } = new List<int>();
        public List<string> IngredientAmounts { get; set; } = new List<string>();
        public List<string> IngredientUnits { get; set; } = new List<string>();
    }
}