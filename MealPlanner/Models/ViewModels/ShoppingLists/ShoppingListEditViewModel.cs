using MealPlanner.Dto;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MealPlanner.Models.ViewModels.ShoppingLists
{
    public class ShoppingListEditViewModel
    {
        public ShoppingListUpdateDto ShoppingList { get; set; } = null!;
        
        // For the Add Item form/modal
        public SelectList AvailableIngredients { get; set; } = null!;
        public ShoppingListItemCreateDto NewItem { get; set; } = new ShoppingListItemCreateDto();
        
        // ✅ Replace ShoppingListItems
        public List<ShoppingListItemDto> ExistingItems { get; set; } = new();

        // ✅ Replace IngredientCategories
        public Dictionary<int, string> IngredientCategories { get; set; } = new();

        // ✅ Replace summary numbers
        public int TotalItems => ExistingItems.Count;
        public int PurchasedItems => ExistingItems.Count(i => i.Purchased);
        public int RemainingItems => TotalItems - PurchasedItems;
    }
}