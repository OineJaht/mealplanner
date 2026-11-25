using MealPlanner.Dto;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace MealPlanner.Models.ViewModels.UserPantry;

public class UserPantryLowStockViewModel
{
    public List<UserPantryItemDto> Items { get; set; } = new();
    public List<SelectListItem> AvailableShoppingLists { get; set; } = new();
    public int TotalLowStockItems => Items.Count;
    public decimal AverageStockLevel => Items.Any() ? Items.Average(i => i.AmountInStock) : 0;
}