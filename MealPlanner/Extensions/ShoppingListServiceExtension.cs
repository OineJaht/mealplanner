using AutoMapper;
using MealPlanner.BLL.Services;
using MealPlanner.Data.UnitOfWork;
using MealPlanner.Dto;

namespace MealPlanner.Extensions
{
    public static class ShoppingListServiceExtensions
    {
        public static async Task<ShoppingListItemDto?> GetItemAsync(
            this IShoppingListService service, 
            int id, 
            string userId,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            // Get the shopping list item
            var shoppingListItem = await unitOfWork.ShoppingListItems.GetByIdAsync(id);
            if (shoppingListItem == null)
            {
                return null;
            }
    
            // Verify that the user owns the associated shopping list
            var shoppingList = await unitOfWork.ShoppingLists.GetByIdAsync(shoppingListItem.ShoppingListId);
            if (shoppingList == null || shoppingList.UserId != userId)
            {
                // User does not own this shopping list
                return null;
            }
    
            // Item exists and user is authorized, map to DTO and return
            return mapper.Map<ShoppingListItemDto>(shoppingListItem);
        }
    }
}