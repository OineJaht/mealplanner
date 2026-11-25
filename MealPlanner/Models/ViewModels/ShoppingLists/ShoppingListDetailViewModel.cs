using System.Collections.Generic;
using MealPlanner.Dto;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MealPlanner.Models.ViewModels.ShoppingLists
{
    public class ShoppingListDetailViewModel
    {
        public ShoppingListDetailDto ShoppingList { get; set; } = null!;
        
        // For the Add Item form/modal
        public SelectList AvailableIngredients { get; set; } = null!;
        public ShoppingListItemCreateDto NewItem { get; set; } = new ShoppingListItemCreateDto();
        // ✅ New: Flattened ingredient info for display
        public List<IngredientListItem> Ingredients { get; set; } = new();

        // ✅ Optional: Category lookup (if needed)
        public Dictionary<int, string> IngredientCategories { get; set; } = new();
    }

    public class IngredientListItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}