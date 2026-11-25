// MealPlanner/BLL/Services/Implementations/ShoppingListService.cs
using AutoMapper;
// using MealPlanner.BLL.DTO;
using MealPlanner.Data.UnitOfWork;
using MealPlanner.Data.Repositories;
using MealPlanner.Models.Inventory;
using System.Linq;

namespace MealPlanner.BLL.Services.Implementations
{
    /// <summary>
    /// Shopping list service implementation
    /// </summary>
    public class ShoppingListService : BaseService<DTO.ShoppingList, Models.Inventory.ShoppingList, int>, IShoppingListService
    {
        /// <summary>
        /// Shopping list repository
        /// </summary>
        private readonly IShoppingListRepository _shoppingListRepository;
        
        /// <summary>
        /// Shopping list item repository
        /// </summary>
        private readonly IRepository<Models.Inventory.ShoppingListItem> _shoppingListItemRepository;
        
        private readonly ILogger<ShoppingListService> _logger; // Add this
        
        /// <summary>
        /// Meal plan repository
        /// </summary>
        private readonly IMealPlanRepository _mealPlanRepository;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork">Unit of work</param>
        /// <param name="mapper">Mapper</param>
        /// <param name="logger">Logger</param> // Add this parameter
        public ShoppingListService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ShoppingListService> logger)
            : base(unitOfWork, mapper, unitOfWork.ShoppingLists)
        {
            _shoppingListRepository = unitOfWork.ShoppingLists;
            _shoppingListItemRepository = unitOfWork.ShoppingListItems;
            _mealPlanRepository = unitOfWork.MealPlans;
            _logger = logger;
        }
        
        public async Task<List<BLL.DTO.ShoppingList>> GetAllAsync(string userId)
        {
            _logger.LogInformation($"SERVICE DEBUG: Getting shopping lists for user {userId}");
    
            // First, let's see what method you're actually calling
            var userLists = await _shoppingListRepository.GetUserShoppingListsWithItemsAsync(userId);
    
            _logger.LogInformation($"SERVICE DEBUG: Found {userLists.Count} lists from repository");
    
            foreach (var list in userLists)
            {
                _logger.LogInformation($"SERVICE DEBUG: List '{list.Name}' (ID: {list.Id}) has {list.Items?.Count ?? 0} items");
                if (list.Items?.Any() == true)
                {
                    foreach (var item in list.Items)
                    {
                        _logger.LogInformation($"SERVICE DEBUG: - Item: {item.Ingredient?.Name ?? "NULL"}, Amount: {item.Amount}");
                    }
                }
            }
    
            var mapped = userLists.Select(sl => Mapper.Map<BLL.DTO.ShoppingList>(sl)).ToList();
    
            _logger.LogInformation($"SERVICE DEBUG: Mapped to {mapped.Count} BLL DTOs");
    
            foreach (var mappedList in mapped)
            {
                _logger.LogInformation($"SERVICE DEBUG: Mapped list '{mappedList.Name}' has {mappedList.Items?.Count ?? 0} items");
            }
    
            return mapped;
        }
        
        public override async Task<List<BLL.DTO.ShoppingList>> AllAsync(string? userId = null)
        {
            Console.WriteLine($"SERVICE DEBUG: AllAsync called with userId: {userId}");
            _logger.LogInformation($"SERVICE DEBUG: AllAsync called with userId: {userId}");
    
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("UserId is required for shopping lists");
            }
    
            // Use the method that includes items
            var userLists = await _shoppingListRepository.GetUserShoppingListsWithItemsAsync(userId);
    
            Console.WriteLine($"SERVICE DEBUG: Found {userLists.Count} lists from repository");
            _logger.LogInformation($"SERVICE DEBUG: Found {userLists.Count} lists from repository");
    
            foreach (var list in userLists)
            {
                Console.WriteLine($"SERVICE DEBUG: List '{list.Name}' (ID: {list.Id}) has {list.Items?.Count ?? 0} items");
                _logger.LogInformation($"SERVICE DEBUG: List '{list.Name}' (ID: {list.Id}) has {list.Items?.Count ?? 0} items");
            }
    
            var mapped = userLists.Select(sl => Mapper.Map<BLL.DTO.ShoppingList>(sl)).ToList();
    
            Console.WriteLine($"SERVICE DEBUG: Mapped to {mapped.Count} BLL DTOs");
            _logger.LogInformation($"SERVICE DEBUG: Mapped to {mapped.Count} BLL DTOs");
    
            foreach (var mappedList in mapped)
            {
                Console.WriteLine($"SERVICE DEBUG: Mapped list '{mappedList.Name}' has {mappedList.Items?.Count ?? 0} items");
                _logger.LogInformation($"SERVICE DEBUG: Mapped list '{mappedList.Name}' has {mappedList.Items?.Count ?? 0} items");
            }
    
            return mapped;
        }

        
        /// <summary>
        /// Apply user filter to query
        /// </summary>
        /// <param name="query">Query to filter</param>
        /// <param name="userId">User ID</param>
        /// <returns>Filtered query</returns>
        protected override IQueryable<Models.Inventory.ShoppingList> ApplyUserFilter(IQueryable<Models.Inventory.ShoppingList> query, string? userId)
        {
            if (userId != null)
            {
                query = query.Where(sl => sl.UserId == userId);
            }
            return query;
        }

        /// <summary>
        /// Verify if user owns this entity
        /// </summary>
        /// <param name="entity">Entity to check</param>
        /// <param name="userId">User ID</param>
        /// <returns>True if user owns this entity</returns>
        protected override bool VerifyOwnership(Models.Inventory.ShoppingList entity, string userId)
        {
            return entity.UserId == userId;
        }

        /// <summary>
        /// Set owner of entity
        /// </summary>
        /// <param name="entity">Entity to set owner</param>
        /// <param name="userId">User ID</param>
        protected override void SetOwner(Models.Inventory.ShoppingList entity, string userId)
        {
            entity.UserId = userId;
            entity.CreatedDate = DateTime.UtcNow;
        }

        /// <summary>
        /// Get shopping list with all items
        /// </summary>
        /// <param name="id">Shopping list ID</param>
        /// <param name="userId">User ID to verify ownership</param>
        /// <returns>Shopping list with items or null</returns>
        public async Task<DTO.ShoppingListDetail?> GetWithItemsAsync(int id, string userId)
        {
            var shoppingList = await _shoppingListRepository.GetShoppingListWithItemsAsync(id);
            
            if (shoppingList == null)
            {
                return null;
            }
            
            // Verify ownership
            if (shoppingList.UserId != userId)
            {
                return null;
            }
            
            return Mapper.Map<DTO.ShoppingListDetail>(shoppingList);
        }

        /// <summary>
        /// Create shopping list from meal plan
        /// </summary>
        /// <param name="mealPlanId">Meal plan ID</param>
        /// <param name="name">New shopping list name</param>
        /// <param name="userId">User ID to verify ownership</param>
        /// <returns>Created shopping list with items</returns>
        public async Task<DTO.ShoppingListDetail> CreateFromMealPlanAsync(int mealPlanId, string name, string userId)
        {
            var mealPlan = await _mealPlanRepository.GetMealPlanWithItemsAsync(mealPlanId);
            
            if (mealPlan == null)
            {
                throw new ArgumentException($"Meal plan with ID {mealPlanId} not found");
            }
            
            // Verify ownership
            if (mealPlan.UserId != userId)
            {
                throw new UnauthorizedAccessException($"User {userId} does not own meal plan with ID {mealPlanId}");
            }
            
            // Create new shopping list
            var shoppingList = new Models.Inventory.ShoppingList
            {
                UserId = userId,
                Name = name,
                CreatedDate = DateTime.UtcNow
            };
            
            UnitOfWork.ShoppingLists.Add(shoppingList);
            await UnitOfWork.SaveChangesAsync();
            
            // Add items from meal plan recipes
            var ingredientDict = new Dictionary<int, decimal>();
            
            foreach (var mealPlanItem in mealPlan.MealPlanItems)
            {
                var recipe = await UnitOfWork.Recipes.GetRecipeWithIngredientsAsync(mealPlanItem.RecipeId);
                
                if (recipe != null && recipe.DefaultServings > 0)
                {
                    foreach (var recipeIngredient in recipe.RecipeIngredients)
                    {
                        decimal adjustedAmount = recipeIngredient.Amount * mealPlanItem.Servings / recipe.DefaultServings;
                        
                        if (ingredientDict.ContainsKey(recipeIngredient.IngredientId))
                        {
                            ingredientDict[recipeIngredient.IngredientId] += adjustedAmount;
                        }
                        else
                        {
                            ingredientDict[recipeIngredient.IngredientId] = adjustedAmount;
                        }
                    }
                }
            }
            
            // Add ingredients to shopping list
            foreach (var kvp in ingredientDict)
            {
                var shoppingListItem = new Models.Inventory.ShoppingListItem
                {
                    ShoppingListId = shoppingList.Id,
                    IngredientId = kvp.Key,
                    Amount = kvp.Value,
                    Purchased = false
                };
                
                UnitOfWork.ShoppingListItems.Add(shoppingListItem);
            }
            
            await UnitOfWork.SaveChangesAsync();
            
            // Get complete shopping list with items
            var createdShoppingList = await _shoppingListRepository.GetShoppingListWithItemsAsync(shoppingList.Id);
            
            return Mapper.Map<DTO.ShoppingListDetail>(createdShoppingList);
        }

        /// <summary>
        /// Add item to shopping list
        /// </summary>
        /// <param name="shoppingListId">Shopping list ID</param>
        /// <param name="item">Item to add</param>
        /// <param name="userId">User ID to verify ownership</param>
        /// <returns>Added item with ID</returns>
        public async Task<DTO.ShoppingListItem> AddItemAsync(int shoppingListId, DTO.ShoppingListItem item, string userId)
        {
            var shoppingList = await _shoppingListRepository.GetByIdAsync(shoppingListId);
            
            if (shoppingList == null)
            {
                throw new ArgumentException($"Shopping list with ID {shoppingListId} not found");
            }
            
            // Verify ownership
            if (shoppingList.UserId != userId)
            {
                throw new UnauthorizedAccessException($"User {userId} does not own shopping list with ID {shoppingListId}");
            }
            
            // Check if item already exists
            var existingItems = await _shoppingListItemRepository
                .ToListAsync(i => i.ShoppingListId == shoppingListId && i.IngredientId == item.IngredientId);
            
            var existingItem = existingItems.FirstOrDefault();
            
            if (existingItem != null)
            {
                // Update existing item
                existingItem.Amount += item.Amount;
                _shoppingListItemRepository.Update(existingItem);
                await UnitOfWork.SaveChangesAsync();
                
                return Mapper.Map<DTO.ShoppingListItem>(existingItem);
            }
            else
            {
                // Create new item
                var shoppingListItem = Mapper.Map<Models.Inventory.ShoppingListItem>(item);
                shoppingListItem.ShoppingListId = shoppingListId;
                
                UnitOfWork.ShoppingListItems.Add(shoppingListItem);
                await UnitOfWork.SaveChangesAsync();
                
                return Mapper.Map<DTO.ShoppingListItem>(shoppingListItem);
            }
        }

        /// <summary>
        /// Update shopping list item
        /// </summary>
        /// <param name="item">Item with updated values</param>
        /// <param name="userId">User ID to verify ownership</param>
        /// <returns>Updated item</returns>
        public async Task<DTO.ShoppingListItem> UpdateItemAsync(DTO.ShoppingListItem item, string userId)
        {
            var shoppingListItem = await _shoppingListItemRepository.GetByIdAsync(item.Id);
            
            if (shoppingListItem == null)
            {
                throw new ArgumentException($"Shopping list item with ID {item.Id} not found");
            }
            
            var shoppingList = await _shoppingListRepository.GetByIdAsync(shoppingListItem.ShoppingListId);
            
            if (shoppingList == null)
            {
                throw new ArgumentException($"Shopping list with ID {shoppingListItem.ShoppingListId} not found");
            }
            
            // Verify ownership
            if (shoppingList.UserId != userId)
            {
                throw new UnauthorizedAccessException($"User {userId} does not own shopping list with ID {shoppingListItem.ShoppingListId}");
            }
            
            // Remove item if amount is zero or negative
            if (item.Amount <= 0)
            {
                _shoppingListItemRepository.Remove(shoppingListItem);
                await UnitOfWork.SaveChangesAsync();
                
                return Mapper.Map<DTO.ShoppingListItem>(shoppingListItem);
            }
            
            // Update item
            Mapper.Map(item, shoppingListItem);
            _shoppingListItemRepository.Update(shoppingListItem);
            await UnitOfWork.SaveChangesAsync();
            
            return Mapper.Map<DTO.ShoppingListItem>(shoppingListItem);
        }

        /// <summary>
        /// Toggle shopping list item purchased status
        /// </summary>
        /// <param name="itemId">Item ID</param>
        /// <param name="purchased">New purchased status</param>
        /// <param name="userId">User ID to verify ownership</param>
        /// <returns>Updated item</returns>
        public async Task<DTO.ShoppingListItem> ToggleItemAsync(int itemId, bool purchased, string userId)
        {
            var shoppingListItem = await _shoppingListItemRepository.GetByIdAsync(itemId);
            
            if (shoppingListItem == null)
            {
                throw new ArgumentException($"Shopping list item with ID {itemId} not found");
            }
            
            var shoppingList = await _shoppingListRepository.GetByIdAsync(shoppingListItem.ShoppingListId);
            
            if (shoppingList == null)
            {
                throw new ArgumentException($"Shopping list with ID {shoppingListItem.ShoppingListId} not found");
            }
            
            // Verify ownership
            if (shoppingList.UserId != userId)
            {
                throw new UnauthorizedAccessException($"User {userId} does not own shopping list with ID {shoppingListItem.ShoppingListId}");
            }
            
            // Update purchased status
            shoppingListItem.Purchased = purchased;
            _shoppingListItemRepository.Update(shoppingListItem);
            await UnitOfWork.SaveChangesAsync();
            
            return Mapper.Map<DTO.ShoppingListItem>(shoppingListItem);
        }

        /// <summary>
        /// Remove shopping list item
        /// </summary>
        /// <param name="itemId">Item ID</param>
        /// <param name="userId">User ID to verify ownership</param>
        /// <returns>True if item was removed</returns>
        public async Task<bool> RemoveItemAsync(int itemId, string userId)
        {
            var shoppingListItem = await _shoppingListItemRepository.GetByIdAsync(itemId);
            
            if (shoppingListItem == null)
            {
                return false;
            }
            
            var shoppingList = await _shoppingListRepository.GetByIdAsync(shoppingListItem.ShoppingListId);
            
            if (shoppingList == null)
            {
                return false;
            }
            
            // Verify ownership
            if (shoppingList.UserId != userId)
            {
                throw new UnauthorizedAccessException($"User {userId} does not own shopping list with ID {shoppingListItem.ShoppingListId}");
            }
            
            _shoppingListItemRepository.Remove(shoppingListItem);
            await UnitOfWork.SaveChangesAsync();
            
            return true;
        }

        /// <summary>
        /// Remove completed shopping list items
        /// </summary>
        /// <param name="shoppingListId">Shopping list ID</param>
        /// <param name="userId">User ID to verify ownership</param>
        /// <returns>Number of items removed</returns>
        public async Task<int> RemoveCompletedItemsAsync(int shoppingListId, string userId)
        {
            var shoppingList = await _shoppingListRepository.GetByIdAsync(shoppingListId);
            
            if (shoppingList == null)
            {
                throw new ArgumentException($"Shopping list with ID {shoppingListId} not found");
            }
            
            // Verify ownership
            if (shoppingList.UserId != userId)
            {
                throw new UnauthorizedAccessException($"User {userId} does not own shopping list with ID {shoppingListId}");
            }
            
            // Get completed items
            var completedItems = await _shoppingListItemRepository
                .ToListAsync(i => i.ShoppingListId == shoppingListId && i.Purchased);
            
            // Remove items
            foreach (var item in completedItems)
            {
                _shoppingListItemRepository.Remove(item);
            }
            
            await UnitOfWork.SaveChangesAsync();
            
            return completedItems.Count;
        }
        
        public async Task<int?> GetOwnerShoppingListIdAsync(int itemId, string userId)
        {
            var item = await UnitOfWork.ShoppingListItems.GetByIdAsync(itemId);
            if (item == null) return null;

            var list = await UnitOfWork.ShoppingLists.GetByIdAsync(item.ShoppingListId);
            if (list == null || list.UserId != userId) return null;

            return list.Id;
        }

        public async Task<(int shoppingListId, bool purchased)?> GetItemOwnerInfoAsync(int itemId, string userId)
        {
            var item = await UnitOfWork.ShoppingListItems.GetByIdAsync(itemId);
            if (item == null) return null;

            var list = await UnitOfWork.ShoppingLists.GetByIdAsync(item.ShoppingListId);
            if (list == null || list.UserId != userId) return null;

            return (item.ShoppingListId, item.Purchased);
        }

        
    }
}