using System;
using System.Threading.Tasks;
using MealPlanner.Data.Repositories;
using MealPlanner.Models.Inventory;

namespace MealPlanner.Data.UnitOfWork
{
    /// <summary>
    /// Unit of Work interface - keeping existing structure for backward compatibility
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        // Existing repositories
        IRecipeRepository Recipes { get; }
        IIngredientRepository Ingredients { get; }
        ICategoryRepository Categories { get; }
        IMealPlanRepository MealPlans { get; }
        IShoppingListRepository ShoppingLists { get; }
        IUserPantryRepository UserPantry { get; }
        IRepository<RecipeCategory> RecipeCategories { get; }
        IRepository<RecipeIngredient> RecipeIngredients { get; }
        IRepository<CookingStep> CookingSteps { get; }
        IRepository<RecipeNutrition> RecipeNutrition { get; }
        IRepository<IngredientNutrition> IngredientNutrition { get; }
        IRepository<MealPlanItem> MealPlanItems { get; }
        IRepository<ShoppingListItem> ShoppingListItems { get; }
    
        // 🔐 SECURE: Controlled access to DbContext
        ApplicationDbContext Context { get; }
    
        // Transaction methods
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}