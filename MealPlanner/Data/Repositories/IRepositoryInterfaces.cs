// MealPlanner.Data/Repositories/IRepositoryInterfaces.cs - Hybrid approach
using System.Collections.Generic;
using System.Threading.Tasks;
using MealPlanner.Models.Inventory;
using MealPlanner.DAL.DTO;

namespace MealPlanner.Data.Repositories
{
    /// <summary>
    /// Recipe repository - hybrid interface that supports both domain entities and DTOs
    /// </summary>
    public interface IRecipeRepository : IRepository<Recipe>
    {
        // Existing methods that return domain entities (for backward compatibility)
        Task<List<Recipe>> GetUserRecipesAsync(string userId);
        Task<Recipe?> GetRecipeWithDetailsAsync(int id);
        Task<List<Recipe>> GetFavoriteRecipesAsync(string userId);
        Task<List<Recipe>> GetRecentRecipesAsync(string userId, int count = 5);
        Task<Recipe?> GetRecipeWithIngredientsAsync(int id);
        Task<Recipe?> GetRecipeWithCookingStepsAsync(int id);
        Task<Recipe?> GetRecipeWithCategoriesAsync(int id);
        Task<List<Recipe>> GetAllAsync();
        Task<List<Recipe>> GetAllWithCategoriesAsync();
        Task<List<Recipe>> GetUserRecipesWithCategoriesAsync(string userId);

        // New DTO-based methods (for future use)
        Task<IEnumerable<RecipeDto>> GetUserRecipesDtoAsync(string userId);
        Task<RecipeDto?> GetRecipeWithDetailsDtoAsync(int id);
        Task<IEnumerable<RecipeDto>> GetFavoriteRecipesDtoAsync(string userId);
        Task<IEnumerable<RecipeDto>> GetRecentRecipesDtoAsync(string userId, int count = 5);
    }
    
    /// <summary>
    /// Ingredient repository - hybrid interface
    /// </summary>
    public interface IIngredientRepository : IRepository<Ingredient>
    {
        // Existing methods that return domain entities
        Task<Ingredient?> GetIngredientWithNutritionAsync(int id);
        Task<Ingredient?> GetIngredientWithRecipesAsync(int id);
        Task<List<Ingredient>> GetIngredientsByCategoryAsync(string category);
        Task<List<Ingredient>> SearchIngredientsAsync(string searchTerm);
        Task<List<Ingredient>> GetAllAsync();

        // New DTO-based methods
        Task<IEnumerable<IngredientDto>> GetIngredientsDtoAsync();
        Task<IngredientDto?> GetIngredientWithNutritionDtoAsync(int id);
    }
    
    /// <summary>
    /// Category repository - hybrid interface
    /// </summary>
    public interface ICategoryRepository : IRepository<Category>
    {
        // Existing methods that return domain entities
        Task<Category?> GetCategoryWithRecipesAsync(int id);
        Task<List<string>> GetAllCategoryNamesAsync();

        // New DTO-based methods
        Task<IEnumerable<CategoryDto>> GetCategoriesDtoAsync();
    }
    
    /// <summary>
    /// Meal plan repository - hybrid interface
    /// </summary>
    public interface IMealPlanRepository : IRepository<MealPlan>
    {
        // Existing methods that return domain entities
        Task<List<MealPlan>> GetUserMealPlansAsync(string userId);
        Task<MealPlan?> GetMealPlanWithItemsAsync(int id);
        Task<MealPlan?> GetCurrentMealPlanAsync(string userId);

        // New DTO-based methods
        Task<IEnumerable<MealPlanDto>> GetUserMealPlansDtoAsync(string userId);
    }
    
    /// <summary>
    /// Shopping list repository - hybrid interface
    /// </summary>
    public interface IShoppingListRepository : IRepository<ShoppingList>
    {
        // Existing methods that return domain entities
        Task<List<ShoppingList>> GetUserShoppingListsAsync(string userId);
        Task<ShoppingList?> GetShoppingListWithItemsAsync(int id);
        Task<List<ShoppingList>> GetAllAsync();

        // New DTO-based methods
        Task<IEnumerable<ShoppingListDto>> GetShoppingListsDtoAsync(string userId);
        Task<List<ShoppingList>> GetUserShoppingListsWithItemsAsync(string userId);
    }
    
    /// <summary>
    /// User pantry repository - hybrid interface
    /// </summary>
    public interface IUserPantryRepository : IRepository<UserPantryItem>
    {
        // Existing methods that return domain entities
        Task<List<UserPantryItem>> GetUserPantryItemsAsync(string userId);
        Task<List<UserPantryItem>> GetExpiringItemsAsync(string userId, int daysThreshold = 7);
        Task<List<UserPantryItem>> GetLowStockItemsAsync(string userId, decimal threshold = 5);
        Task<List<Ingredient>> GetUserPantryIngredientsAsync(string userId);

        // New DTO-based methods
        Task<IEnumerable<UserPantryItemDto>> GetUserPantryItemsDtoAsync(string userId);
    }
}