using MealPlanner.Dto;
namespace MealPlanner.Models.ViewModels.ShoppingLists;
public class ShoppingListIndexViewModel
{
    public List<ShoppingListDto> Lists { get; set; } = new();

    // Key: list ID → value: count
    public Dictionary<int, int> TotalItemsPerList { get; set; } = new();
    public Dictionary<int, int> PurchasedItemsPerList { get; set; } = new();
    public Dictionary<int, List<ShoppingListItemDto>> TopItems { get; set; } = new();

}