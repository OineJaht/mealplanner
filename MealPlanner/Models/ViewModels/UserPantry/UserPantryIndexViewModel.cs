using MealPlanner.Dto;

namespace MealPlanner.Models.ViewModels.UserPantry;

public class UserPantryIndexViewModel
{
    public List<UserPantryItemDto> Items { get; set; } = new();
    public int TotalItems => Items.Count;
    public int ExpiringCount => Items.Count(i => (i.ExpiryDate - DateTime.Today).Days <= 7);
    public int LowStockCount { get; set; } // You could calculate this if needed
}