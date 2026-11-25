using MealPlanner.Dto;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MealPlanner.Models.ViewModels.UserPantry
{
    public class UserPantryDetailsViewModel
    {
        public UserPantryItemDto PantryItem { get; set; } = new UserPantryItemDto();
        public string IngredientCategory { get; set; } = string.Empty;
        public IngredientNutritionDto? Nutrition { get; set; }
        public IEnumerable<SelectListItem> AvailableShoppingLists { get; set; } = new List<SelectListItem>();
        public int? SelectedShoppingListId { get; set; }

    }
}