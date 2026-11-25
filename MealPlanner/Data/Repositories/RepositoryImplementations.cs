// MealPlanner.Data/Repositories/RepositoryImplementations.cs - Complete implementation
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MealPlanner.Models.Inventory;
using MealPlanner.DAL.DTO;

namespace MealPlanner.Data.Repositories
{
    /// <summary>
    /// Recipe repository implementation - supports both domain entities and DTOs
    /// </summary>
    public class RecipeRepository : Repository<Recipe>, IRecipeRepository
    {
        private readonly IMapper? _mapper;

        public RecipeRepository(ApplicationDbContext context) : base(context)
        {
            // For now, we'll work without mapper until it's properly configured
        }

        public RecipeRepository(ApplicationDbContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }
        
        // Existing methods that return domain entities (unchanged for backward compatibility)
        public async Task<List<Recipe>> GetAllAsync()
        {
            return await _context.Recipes
                .Include(r => r.RecipeCategories)
                .ThenInclude(rc => rc.Category)
                .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
                .ToListAsync();
        }

        public async Task<List<Recipe>> GetAllWithCategoriesAsync()
        {
            return await _context.Recipes
                .Include(r => r.RecipeCategories)
                .ThenInclude(rc => rc.Category)
                .ToListAsync();
        }

        public async Task<List<Recipe>> GetUserRecipesWithCategoriesAsync(string userId)
        {
            return await _context.Recipes
                .Where(r => r.UserId == userId)
                .Include(r => r.RecipeCategories)
                .ThenInclude(rc => rc.Category)
                .ToListAsync();
        }

        public async Task<List<Recipe>> GetUserRecipesAsync(string userId)
        {
            return await _context.Recipes
                .Where(r => r.UserId == userId)
                .Include(r => r.RecipeCategories)
                .ThenInclude(rc => rc.Category)
                .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
                .ToListAsync();
        }

        public async Task<Recipe?> GetRecipeWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(r => r.User)
                .Include(r => r.RecipeNutrition)
                .Include(r => r.RecipeCategories)
                    .ThenInclude(rc => rc.Category)
                .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Ingredient)
                .Include(r => r.CookingSteps)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<List<Recipe>> GetFavoriteRecipesAsync(string userId)
        {
            return await _dbSet
                .Where(r => r.UserId == userId && r.IsFavorite)
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        public async Task<List<Recipe>> GetRecentRecipesAsync(string userId, int count = 5)
        {
            return await _dbSet
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<Recipe?> GetRecipeWithIngredientsAsync(int id)
        {
            return await _dbSet
                .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Ingredient)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Recipe?> GetRecipeWithCookingStepsAsync(int id)
        {
            return await _dbSet
                .Include(r => r.CookingSteps.OrderBy(cs => cs.StepNumber))
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Recipe?> GetRecipeWithCategoriesAsync(int id)
        {
            return await _dbSet
                .Include(r => r.RecipeCategories)
                    .ThenInclude(rc => rc.Category)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        // New DTO-based methods (for future use when mapper is configured)
        public async Task<IEnumerable<RecipeDto>> GetUserRecipesDtoAsync(string userId)
        {
            if (_mapper == null) 
                throw new InvalidOperationException("Mapper not configured for DTO operations");

            var recipes = await GetUserRecipesAsync(userId);
            return _mapper.Map<IEnumerable<RecipeDto>>(recipes);
        }

        public async Task<RecipeDto?> GetRecipeWithDetailsDtoAsync(int id)
        {
            if (_mapper == null) 
                throw new InvalidOperationException("Mapper not configured for DTO operations");

            var recipe = await GetRecipeWithDetailsAsync(id);
            return recipe == null ? null : _mapper.Map<RecipeDto>(recipe);
        }

        public async Task<IEnumerable<RecipeDto>> GetFavoriteRecipesDtoAsync(string userId)
        {
            if (_mapper == null) 
                throw new InvalidOperationException("Mapper not configured for DTO operations");

            var recipes = await GetFavoriteRecipesAsync(userId);
            return _mapper.Map<IEnumerable<RecipeDto>>(recipes);
        }

        public async Task<IEnumerable<RecipeDto>> GetRecentRecipesDtoAsync(string userId, int count = 5)
        {
            if (_mapper == null) 
                throw new InvalidOperationException("Mapper not configured for DTO operations");

            var recipes = await GetRecentRecipesAsync(userId, count);
            return _mapper.Map<IEnumerable<RecipeDto>>(recipes);
        }
    }

    /// <summary>
    /// Ingredient repository implementation - hybrid approach
    /// </summary>
    public class IngredientRepository : Repository<Ingredient>, IIngredientRepository
    {
        private readonly IMapper? _mapper;

        public IngredientRepository(ApplicationDbContext context) : base(context)
        {
        }

        public IngredientRepository(ApplicationDbContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }
        
        // Existing methods (unchanged)
        public async Task<List<Ingredient>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<Ingredient?> GetIngredientWithNutritionAsync(int id)
        {
            return await _dbSet
                .Include(i => i.IngredientNutrition)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Ingredient?> GetIngredientWithRecipesAsync(int id)
        {
            return await _dbSet
                .Include(i => i.RecipeIngredients)
                    .ThenInclude(ri => ri.Recipe)
                .Include(i => i.UserPantryItems)
                .Include(i => i.ShoppingListItems)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<List<Ingredient>> GetIngredientsByCategoryAsync(string category)
        {
            return await _dbSet
                .Where(i => i.Category == category)
                .OrderBy(i => i.Name)
                .ToListAsync();
        }

        public async Task<List<Ingredient>> SearchIngredientsAsync(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
                return new List<Ingredient>();

            return await _dbSet
                .Where(i => i.Name.Contains(searchTerm) || i.Category.Contains(searchTerm))
                .OrderBy(i => i.Name)
                .ToListAsync();
        }

        // New DTO-based methods
        public async Task<IEnumerable<IngredientDto>> GetIngredientsDtoAsync()
        {
            if (_mapper == null) 
                throw new InvalidOperationException("Mapper not configured for DTO operations");

            var ingredients = await GetAllAsync();
            return _mapper.Map<IEnumerable<IngredientDto>>(ingredients);
        }

        public async Task<IngredientDto?> GetIngredientWithNutritionDtoAsync(int id)
        {
            if (_mapper == null) 
                throw new InvalidOperationException("Mapper not configured for DTO operations");

            var ingredient = await GetIngredientWithNutritionAsync(id);
            return ingredient == null ? null : _mapper.Map<IngredientDto>(ingredient);
        }
    }

    /// <summary>
    /// Category repository implementation - hybrid approach
    /// </summary>
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly IMapper? _mapper;

        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public CategoryRepository(ApplicationDbContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }

        public async Task<Category?> GetCategoryWithRecipesAsync(int id)
        {
            return await _dbSet
                .Include(c => c.RecipeCategories)
                    .ThenInclude(rc => rc.Recipe)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<string>> GetAllCategoryNamesAsync()
        {
            return await _dbSet
                .Select(c => c.Name)
                .OrderBy(n => n)
                .ToListAsync();
        }

        // New DTO-based methods
        public async Task<IEnumerable<CategoryDto>> GetCategoriesDtoAsync()
        {
            if (_mapper == null) 
                throw new InvalidOperationException("Mapper not configured for DTO operations");

            var categories = await _dbSet.ToListAsync();
            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }
    }

    /// <summary>
    /// Meal plan repository implementation - hybrid approach
    /// </summary>
    public class MealPlanRepository : Repository<MealPlan>, IMealPlanRepository
    {
        private readonly IMapper? _mapper;

        public MealPlanRepository(ApplicationDbContext context) : base(context)
        {
        }

        public MealPlanRepository(ApplicationDbContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }

        public async Task<List<MealPlan>> GetUserMealPlansAsync(string userId)
        {
            return await _dbSet
                .Where(mp => mp.UserId == userId)
                .OrderByDescending(mp => mp.StartDate)
                .ToListAsync();
        }

        public async Task<MealPlan?> GetMealPlanWithItemsAsync(int id)
        {
            var result = await _dbSet
                .Include(mp => mp.User)
                .Include(mp => mp.MealPlanItems)
                .ThenInclude(mpi => mpi.Recipe)
                .FirstOrDefaultAsync(mp => mp.Id == id);

            // Add debug logging
            if (result != null)
            {
                Console.WriteLine($"REPO DEBUG: Found meal plan '{result.Name}' with {result.MealPlanItems?.Count ?? 0} items");
                if (result.MealPlanItems?.Any() == true)
                {
                    foreach (var item in result.MealPlanItems)
                    {
                        Console.WriteLine($"REPO DEBUG: Item {item.Id}: Recipe={item.Recipe?.Name ?? "NULL"}, Type={item.MealType}, Date={item.PlannedDate}, Servings={item.Servings}");
                    }
                }
            }
            else
            {
                Console.WriteLine($"REPO DEBUG: No meal plan found with ID {id}");
            }

            return result;
        }

        public async Task<MealPlan?> GetCurrentMealPlanAsync(string userId)
        {
            var today = DateTime.UtcNow.Date;
            return await _dbSet
                .Where(mp => mp.UserId == userId && 
                             mp.StartDate <= today && 
                             mp.EndDate >= today)
                .OrderBy(mp => mp.StartDate)
                .Include(mp => mp.MealPlanItems)
                    .ThenInclude(mpi => mpi.Recipe)
                .FirstOrDefaultAsync();
        }

        // New DTO-based methods
        public async Task<IEnumerable<MealPlanDto>> GetUserMealPlansDtoAsync(string userId)
        {
            if (_mapper == null) 
                throw new InvalidOperationException("Mapper not configured for DTO operations");

            var mealPlans = await GetUserMealPlansAsync(userId);
            return _mapper.Map<IEnumerable<MealPlanDto>>(mealPlans);
        }
    }

    /// <summary>
    /// Shopping list repository implementation - hybrid approach
    /// </summary>
    public class ShoppingListRepository : Repository<ShoppingList>, IShoppingListRepository
    {
        private readonly IMapper? _mapper;

        public ShoppingListRepository(ApplicationDbContext context) : base(context)
        {
        }

        public ShoppingListRepository(ApplicationDbContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }

        public async Task<List<ShoppingList>> GetUserShoppingListsAsync(string userId)
        {
            return await _dbSet
                .Where(sl => sl.UserId == userId)
                .OrderByDescending(sl => sl.CreatedDate)
                .ToListAsync();
        }
        
        public async Task<List<ShoppingList>> GetAllAsync()
        {
            return await _dbSet.ToListAsync(); 
        }

        public async Task<ShoppingList?> GetShoppingListWithItemsAsync(int id)
        {
            return await _dbSet
                .Include(sl => sl.User)
                .Include(sl => sl.Items)
                    .ThenInclude(i => i.Ingredient)
                .FirstOrDefaultAsync(sl => sl.Id == id);
        }

        // New DTO-based methods
        public async Task<IEnumerable<ShoppingListDto>> GetShoppingListsDtoAsync(string userId)
        {
            if (_mapper == null) 
                throw new InvalidOperationException("Mapper not configured for DTO operations");

            var shoppingLists = await GetUserShoppingListsAsync(userId);
            return _mapper.Map<IEnumerable<ShoppingListDto>>(shoppingLists);
        }
        public async Task<List<ShoppingList>> GetUserShoppingListsWithItemsAsync(string userId)
        {
            return await _dbSet
                .Where(sl => sl.UserId == userId)
                .Include(sl => sl.Items)
                .ThenInclude(i => i.Ingredient)
                .OrderByDescending(sl => sl.CreatedDate)
                .ToListAsync();
        }
    }

    /// <summary>
    /// User pantry repository implementation - hybrid approach
    /// </summary>
    public class UserPantryRepository : Repository<UserPantryItem>, IUserPantryRepository
    {
        private readonly IMapper? _mapper;

        public UserPantryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public UserPantryRepository(ApplicationDbContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }

        public async Task<List<UserPantryItem>> GetUserPantryItemsAsync(string userId)
        {
            return await _dbSet
                .Include(p => p.Ingredient)
                .Where(p => p.UserId == userId)
                .ToListAsync();
        }

        public async Task<List<UserPantryItem>> GetExpiringItemsAsync(string userId, int daysThreshold = 7)
        {
            var expiryDate = DateTime.UtcNow.AddDays(daysThreshold);
            return await _dbSet
                .Include(p => p.Ingredient)
                .Where(p => p.UserId == userId && p.ExpiryDate <= expiryDate)
                .OrderBy(p => p.ExpiryDate)
                .ToListAsync();
        }

        public async Task<List<UserPantryItem>> GetLowStockItemsAsync(string userId, decimal threshold = 5)
        {
            return await _dbSet
                .Include(p => p.Ingredient)
                .Where(p => p.UserId == userId && p.AmountInStock < threshold)
                .OrderBy(p => p.AmountInStock)
                .ToListAsync();
        }
        
        public async Task<List<Ingredient>> GetUserPantryIngredientsAsync(string userId)
        {
            return await _dbSet
                .Where(p => p.UserId == userId && p.AmountInStock > 0)
                .Select(p => p.Ingredient)
                .Distinct()
                .ToListAsync();
        }

        // New DTO-based methods
        public async Task<IEnumerable<UserPantryItemDto>> GetUserPantryItemsDtoAsync(string userId)
        {
            if (_mapper == null) 
                throw new InvalidOperationException("Mapper not configured for DTO operations");

            var pantryItems = await GetUserPantryItemsAsync(userId);
            return _mapper.Map<IEnumerable<UserPantryItemDto>>(pantryItems);
        }
    }
}