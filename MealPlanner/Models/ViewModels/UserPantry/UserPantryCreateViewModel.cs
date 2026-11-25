using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using MealPlanner.Dto;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MealPlanner.Models.ViewModels.UserPantry
{
    public class UserPantryCreateViewModel
    {
        public UserPantryItemCreateDto PantryItem { get; set; } = new UserPantryItemCreateDto();
        
        [ValidateNever]
        public SelectList AvailableIngredients { get; set; } = null!;
    }
}