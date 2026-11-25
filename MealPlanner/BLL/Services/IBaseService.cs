// MealPlanner.BLL/Services/IBaseService.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MealPlanner.BLL.DTO;
using MealPlanner.Dto;
using MealPlanner.Models.Inventory.ViewModels.Recipes;

namespace MealPlanner.BLL.Services
{
    /// <summary>
    /// Base service interface for all BLL services
    /// </summary>
    /// <typeparam name="TEntity">Type of entity this service manages</typeparam>
    /// <typeparam name="TKey">Type of the primary key</typeparam>
    public interface IBaseService<TEntity, TKey> where TEntity : class
    {
        /// <summary>
        /// Get all entities
        /// </summary>
        /// <param name="userId">Optional user ID to filter by</param>
        /// <returns>List of entities</returns>
        Task<List<TEntity>> AllAsync(string? userId = null);

        /// <summary>
        /// Find entity by ID
        /// </summary>
        /// <param name="id">Entity ID</param>
        /// <param name="userId">Optional user ID to verify ownership</param>
        /// <returns>Entity or null if not found</returns>
        Task<TEntity?> FindAsync(TKey id, string? userId = null);

        /// <summary>
        /// Add new entity
        /// </summary>
        /// <param name="entity">Entity to add</param>
        /// <param name="userId">User ID who owns this entity</param>
        /// <returns>Added entity with ID</returns>
        Task<TEntity> AddAsync(TEntity entity, string userId);

        /// <summary>
        /// Update existing entity
        /// </summary>
        /// <param name="entity">Entity with updated values</param>
        /// <param name="userId">User ID to verify ownership</param>
        /// <returns>Updated entity</returns>
        Task<TEntity> UpdateAsync(TEntity entity, string userId);

        /// <summary>
        /// Remove entity by ID
        /// </summary>
        /// <param name="id">Entity ID</param>
        /// <param name="userId">User ID to verify ownership</param>
        /// <returns>True if entity was removed</returns>
        Task<bool> RemoveAsync(TKey id, string userId);
    }

    /// <summary>
    /// Recipe service interface for BLL operations on recipes
    /// </summary>
    public interface IRecipeService : IBaseService<MealPlanner.BLL.DTO.Recipe, int>
    {
        /// <summary>
        /// Get recipe with all related details
        /// </summary>
        /// <param name="id">Recipe ID</param>
        /// <param name="userId">User ID to verify ownership</param>
        /// <returns>Recipe with details or null</returns>
        Task<MealPlanner.BLL.DTO.RecipeDetail?> GetWithDetailsAsync(int id, string? userId = null);
        
        Task<Recipe> AddFullRecipeAsync(
            RecipeCreateDto recipeDto,
            string userId,
            int[] categoryIds,
            List<(int ingredientId, decimal amount, string unit)> ingredients);

        Task<MealPlanner.BLL.DTO.Recipe> UpdateFullRecipeAsync(
            RecipeUpdateDto recipeDto,
            string userId,
            List<int> categoryIds,
            Dictionary<int, RecipeIngredientInfo> ingredients);
        

        /// <summary>
        /// Get favorite recipes for user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of favorite recipes</returns>
        Task<List<MealPlanner.BLL.DTO.Recipe>> GetFavoritesAsync(string userId);

        /// <summary>
        /// Get recent recipes for user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="count">Number of recipes to return</param>
        /// <returns>List of recent recipes</returns>
        Task<List<MealPlanner.BLL.DTO.Recipe>> GetRecentAsync(string userId, int count = 5);
        
        /// <summary>
        /// Update or create recipe nutrition information
        /// </summary>
        /// <param name="nutrition">Recipe nutrition data</param>
        /// <param name="userId">User ID to verify ownership</param>
        /// <returns>Updated recipe nutrition</returns>
        Task<BLL.DTO.RecipeNutrition> UpdateNutritionAsync(BLL.DTO.RecipeNutrition nutrition, string userId);

        /// <summary>
        /// Calculate recipe nutrition based on ingredients
        /// </summary>
        /// <param name="recipeId">Recipe ID</param>
        /// <param name="userId">User ID to verify ownership</param>
        /// <returns>Calculated recipe nutrition</returns>
        Task<BLL.DTO.RecipeNutrition> CalculateNutritionAsync(int recipeId, string userId);
        
        Task<CookingStep?> GetCookingStepAsync(int id, string userId);
        Task<CookingStep> AddCookingStepAsync(CookingStep step, string userId);
        Task<CookingStep> UpdateCookingStepAsync(CookingStep step, string userId);
        Task<bool> RemoveCookingStepAsync(int id, string userId);
        Task ReorderCookingStepsAsync(int recipeId, string userId);
        Task ReorderCookingStepsAsync(int recipeId, int[] stepIds, string userId);
    }

    /// <summary>
    /// Ingredient service interface for BLL operations on ingredients
    /// </summary>
    public interface IIngredientService : IBaseService<MealPlanner.BLL.DTO.Ingredient, int>
    {
        /// <summary>
        /// Get ingredient with nutrition information
        /// </summary>
        /// <param name="id">Ingredient ID</param>
        /// <returns>Ingredient with nutrition or null</returns>
        Task<MealPlanner.BLL.DTO.IngredientDetail?> GetWithNutritionAsync(int id);

        /// <summary>
        /// Search ingredients by name or category
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <returns>List of matching ingredients</returns>
        Task<List<MealPlanner.BLL.DTO.Ingredient>> SearchAsync(string searchTerm);

        /// <summary>
        /// Get ingredients by category
        /// </summary>
        /// <param name="category">Category name</param>
        /// <returns>List of ingredients in category</returns>
        Task<List<MealPlanner.BLL.DTO.Ingredient>> GetByCategoryAsync(string category);
        
        /// <summary>
        /// Add nutrition information to an ingredient
        /// </summary>
        /// <param name="ingredientId">Ingredient ID</param>
        /// <param name="nutrition">Nutrition data</param>
        /// <param name="userId">User ID</param>
        /// <returns>Created ingredient nutrition</returns>
        Task<BLL.DTO.IngredientNutrition> AddNutritionAsync(int ingredientId, BLL.DTO.IngredientNutrition nutrition, string userId);

        /// <summary>
        /// Update ingredient nutrition information
        /// </summary>
        /// <param name="nutrition">Nutrition data</param>
        /// <param name="userId">User ID</param>
        /// <returns>Updated ingredient nutrition</returns>
        Task<BLL.DTO.IngredientNutrition> UpdateNutritionAsync(BLL.DTO.IngredientNutrition nutrition, string userId);
        
        Task<List<Ingredient>> GetAllAsync();
        
    }

    /// <summary>
    /// Interface for category service
    /// </summary>
    public interface ICategoryService : IBaseService<Category, int>
    {
        /// <summary>
        /// Get all category names
        /// </summary>
        /// <returns>List of category names</returns>
        Task<List<string>> GetAllNamesAsync();
        
        /// <summary>
        /// Get count of recipes in a category
        /// </summary>
        /// <param name="categoryId">Category ID</param>
        /// <returns>Number of recipes in the category</returns>
        Task<int> GetRecipeCountAsync(int categoryId);
        
        /// <summary>
        /// Get all recipes in a category
        /// </summary>
        /// <param name="categoryId">Category ID</param>
        /// <returns>List of recipes in the category</returns>
        Task<List<Recipe>> GetRecipesAsync(int categoryId);
    }

    /// <summary>
    /// Meal plan service interface for BLL operations on meal plans
    /// </summary>
    public interface IMealPlanService : IBaseService<MealPlanner.BLL.DTO.MealPlan, int>
    {
        /// <summary>
        /// Get meal plan with all items
        /// </summary>
        /// <param name="id">Meal plan ID</param>
        /// <param name="userId">User ID to verify ownership</param>
        /// <returns>Meal plan with items or null</returns>
        Task<MealPlanner.BLL.DTO.MealPlanDetail?> GetWithItemsAsync(int id, string userId);
        
        /// <summary>
        /// Get meal plan item by ID with ownership verification
        /// </summary>
        /// <param name="itemId">Meal plan item ID</param>
        /// <param name="userId">User ID to verify ownership</param>
        /// <returns>Meal plan item or null if not found/not owned</returns>
        Task<MealPlanItem?> GetMealPlanItemAsync(int itemId, string userId);

        /// <summary>
        /// Get meal plan that contains a specific item
        /// </summary>
        /// <param name="itemId">Meal plan item ID</param>
        /// <param name="userId">User ID to verify ownership</param>
        /// <returns>Meal plan or null if not found/not owned</returns>
        Task<MealPlan?> GetMealPlanByItemIdAsync(int itemId, string userId);

        /// <summary>
        /// Get current meal plan for user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>Current meal plan or null</returns>
        Task<MealPlanner.BLL.DTO.MealPlanDetail?> GetCurrentAsync(string userId);

        /// <summary>
        /// Add item to meal plan
        /// </summary>
        /// <param name="mealPlanId">Meal plan ID</param>
        /// <param name="item">Item to add</param>
        /// <param name="userId">User ID to verify ownership</param>
        /// <returns>Added item with ID</returns>
        Task<MealPlanner.BLL.DTO.MealPlanItem> AddItemAsync(int mealPlanId, MealPlanner.BLL.DTO.MealPlanItem item, string userId);

        /// <summary>
        /// Update meal plan item
        /// </summary>
        /// <param name="item">Item with updated values</param>
        /// <param name="userId">User ID to verify ownership</param>
        /// <returns>Updated item</returns>
        Task<MealPlanner.BLL.DTO.MealPlanItem> UpdateItemAsync(MealPlanner.BLL.DTO.MealPlanItem item, string userId);

        /// <summary>
        /// Remove meal plan item
        /// </summary>
        /// <param name="itemId">Item ID</param>
        /// <param name="userId">User ID to verify ownership</param>
        /// <returns>True if item was removed</returns>
        Task<bool> RemoveItemAsync(int itemId, string userId);
        
        /// <summary>
        /// Get nutrition summary for all meal plans
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of meal plans with nutrition data</returns>
        Task<List<BLL.DTO.MealPlanDetail>> GetNutritionSummaryAsync(string userId);
        
    }

    /// <summary>
    /// Shopping list service interface for BLL operations on shopping lists
    /// </summary>
    public interface IShoppingListService : IBaseService<MealPlanner.BLL.DTO.ShoppingList, int>
    {
        /// <summary>
        /// Get shopping list with all items
        /// </summary>
        /// <param name="id">Shopping list ID</param>
        /// <param name="userId">User ID to verify ownership</param>
        /// <returns>Shopping list with items or null</returns>
        Task<MealPlanner.BLL.DTO.ShoppingListDetail?> GetWithItemsAsync(int id, string userId);
        
        Task<int?> GetOwnerShoppingListIdAsync(int itemId, string userId);
        
        Task<List<ShoppingList>> GetAllAsync(string userId);
        
        Task<(int shoppingListId, bool purchased)?> GetItemOwnerInfoAsync(int itemId, string userId);
        
        /// <summary>
        /// Create shopping list from meal plan
        /// </summary>
        /// <param name="mealPlanId">Meal plan ID</param>
        /// <param name="name">New shopping list name</param>
        /// <param name="userId">User ID to verify ownership</param>
        /// <returns>Created shopping list with items</returns>
        Task<MealPlanner.BLL.DTO.ShoppingListDetail> CreateFromMealPlanAsync(int mealPlanId, string name, string userId);

        /// <summary>
        /// Add item to shopping list
        /// </summary>
        /// <param name="shoppingListId">Shopping list ID</param>
        /// <param name="item">Item to add</param>
        /// <param name="userId">User ID to verify ownership</param>
        /// <returns>Added item with ID</returns>
        Task<MealPlanner.BLL.DTO.ShoppingListItem> AddItemAsync(int shoppingListId, MealPlanner.BLL.DTO.ShoppingListItem item, string userId);

        /// <summary>
        /// Update shopping list item
        /// </summary>
        /// <param name="item">Item with updated values</param>
        /// <param name="userId">User ID to verify ownership</param>
        /// <returns>Updated item</returns>
        Task<MealPlanner.BLL.DTO.ShoppingListItem> UpdateItemAsync(MealPlanner.BLL.DTO.ShoppingListItem item, string userId);

        /// <summary>
        /// Toggle shopping list item purchased status
        /// </summary>
        /// <param name="itemId">Item ID</param>
        /// <param name="purchased">New purchased status</param>
        /// <param name="userId">User ID to verify ownership</param>
        /// <returns>Updated item</returns>
        Task<MealPlanner.BLL.DTO.ShoppingListItem> ToggleItemAsync(int itemId, bool purchased, string userId);

        /// <summary>
        /// Remove shopping list item
        /// </summary>
        /// <param name="itemId">Item ID</param>
        /// <param name="userId">User ID to verify ownership</param>
        /// <returns>True if item was removed</returns>
        Task<bool> RemoveItemAsync(int itemId, string userId);

        /// <summary>
        /// Remove completed shopping list items
        /// </summary>
        /// <param name="shoppingListId">Shopping list ID</param>
        /// <param name="userId">User ID to verify ownership</param>
        /// <returns>Number of items removed</returns>
        Task<int> RemoveCompletedItemsAsync(int shoppingListId, string userId);
    }

    /// <summary>
    /// User pantry service interface for BLL operations on user pantry items
    /// </summary>
    public interface IUserPantryService : IBaseService<MealPlanner.BLL.DTO.UserPantryItem, int>
    {
        /// <summary>
        /// Get user pantry items
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of pantry items</returns>
        Task<List<MealPlanner.BLL.DTO.UserPantryItem>> GetUserItemsAsync(string userId);

        /// <summary>
        /// Get expiring items
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="daysThreshold">Days until expiration</param>
        /// <returns>List of expiring items</returns>
        Task<List<MealPlanner.BLL.DTO.UserPantryItem>> GetExpiringItemsAsync(string userId, int daysThreshold = 7);

        /// <summary>
        /// Get low stock items
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="threshold">Amount threshold</param>
        /// <returns>List of low stock items</returns>
        Task<List<MealPlanner.BLL.DTO.UserPantryItem>> GetLowStockItemsAsync(string userId, decimal threshold = 5);

        /// <summary>
        /// Get all pantry ingredients for user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of ingredients in pantry</returns>
        Task<List<MealPlanner.BLL.DTO.Ingredient>> GetUserIngredientsAsync(string userId);
    }

    /// <summary>
    /// Recipe finder service for finding recipes based on available ingredients
    /// </summary>
    public interface IRecipeFinderService
    {
        /// <summary>
        /// Find recipes matching ingredients
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="ingredientIds">Available ingredient IDs</param>
        /// <param name="matchAll">Require all ingredients to match</param>
        /// <returns>List of matching recipes with match percentage</returns>
        Task<List<MealPlanner.BLL.DTO.RecipeMatch>> FindRecipesAsync(string userId, List<int> ingredientIds, bool matchAll = false);

        /// <summary>
        /// Get recipe details with available ingredients highlighted
        /// </summary>
        /// <param name="recipeId">Recipe ID</param>
        /// <param name="availableIngredientIds">Available ingredient IDs</param>
        /// <param name="userId">User ID to verify ownership</param>
        /// <returns>Recipe details with availability</returns>
        Task<MealPlanner.BLL.DTO.RecipeWithAvailability?> GetRecipeWithAvailabilityAsync(int recipeId, List<int> availableIngredientIds, string userId);
    }
    
    

    /// <summary>
    /// Main service interface for BLL services
    /// </summary>
    public interface IAppBLL
    {
        IRecipeService Recipes { get; }
        IIngredientService Ingredients { get; }
        ICategoryService Categories { get; }
        IMealPlanService MealPlans { get; }
        IShoppingListService ShoppingLists { get; }
        IUserPantryService UserPantry { get; }
        IRecipeFinderService RecipeFinder { get; }

        /// <summary>
        /// Save all changes
        /// </summary>
        Task<int> SaveChangesAsync();
    }
    
    
}