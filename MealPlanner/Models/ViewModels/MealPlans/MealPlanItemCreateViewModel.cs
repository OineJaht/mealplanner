using System;
using MealPlanner.Dto;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MealPlanner.Models.ViewModels.MealPlans
{
    public class MealPlanItemCreateViewModel
    {
        public MealPlanItemCreateDto MealPlanItem { get; set; } = new MealPlanItemCreateDto();

        public string MealPlanName { get; set; } = string.Empty;
        public int MealPlanId { get; set; }
        // Data that was previously in ViewData
        [ValidateNever]
        
        public SelectList AvailableRecipes { get; set; } = null!;
    }
}