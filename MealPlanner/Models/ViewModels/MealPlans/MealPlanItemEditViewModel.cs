using System;
using MealPlanner.Dto;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MealPlanner.Models.ViewModels.MealPlans
{
    public class MealPlanItemEditViewModel
    {
        public MealPlanItemUpdateDto MealPlanItem { get; set; } = new MealPlanItemUpdateDto();
        
        // Data that was previously in ViewData
        public int MealPlanId { get; set; }
        public string MealPlanName { get; set; } = string.Empty;
        public SelectList AvailableRecipes { get; set; } = null!;
    }
}