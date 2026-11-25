using MealPlanner.Dto;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MealPlanner.Models.ViewModels.UserPantry
{
    public class ExpiringPantryItemsViewModel
    {
        /// <summary>
        /// List of expiring pantry items
        /// </summary>
        public List<UserPantryItemDto> Items { get; set; } = new();
        
        /// <summary>
        /// Available shopping lists for dropdown
        /// </summary>
        public List<SelectListItem> AvailableShoppingLists { get; set; } = new();
        
        /// <summary>
        /// Count of items that have already expired
        /// </summary>
        public int ExpiredItemsCount => Items.Count(i => (i.ExpiryDate - DateTime.Today).Days < 0);
        
        /// <summary>
        /// Count of items expiring within the next 7 days
        /// </summary>
        public int ExpiringSoonCount => Items.Count(i => 
        {
            var daysUntilExpiry = (i.ExpiryDate - DateTime.Today).Days;
            return daysUntilExpiry >= 0 && daysUntilExpiry <= 7;
        });
        
        /// <summary>
        /// Total value estimate of expiring items (if we had cost tracking)
        /// </summary>
        public decimal EstimatedWasteValue => 0; // Could be calculated if we track item costs
        
        /// <summary>
        /// Quick stats for the view
        /// </summary>
        public ExpiringItemsStats Stats => new ExpiringItemsStats
        {
            TotalExpiring = Items.Count,
            AlreadyExpired = ExpiredItemsCount,
            ExpiringSoon = ExpiringSoonCount,
            MostCommonCategory = Items
                .Where(i => !string.IsNullOrEmpty(i.IngredientCategory))
                .GroupBy(i => i.IngredientCategory)
                .OrderByDescending(g => g.Count())
                .FirstOrDefault()?.Key ?? "Unknown"
        };
    }
    
    /// <summary>
    /// Statistics about expiring items
    /// </summary>
    public class ExpiringItemsStats
    {
        public int TotalExpiring { get; set; }
        public int AlreadyExpired { get; set; }
        public int ExpiringSoon { get; set; }
        public string MostCommonCategory { get; set; } = string.Empty;
    }
}