using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using MealPlanner.Dto;

namespace MealPlanner.Models.ViewModels.UserPantry
{
    public class UserPantryEditViewModel
    {
        public UserPantryItemUpdateDto PantryItem { get; set; } = new UserPantryItemUpdateDto();
        public SelectList AvailableIngredients { get; set; } = null!;
        public IEnumerable<SelectListItem> AvailableShoppingLists { get; set; } = new List<SelectListItem>();
    }
}