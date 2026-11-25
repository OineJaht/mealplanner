using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MealPlanner.Data.Repositories;
using MealPlanner.Models.Inventory;


namespace MealPlanner.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;
        private bool _disposed = false;

        // Repositories
        private IRecipeRepository? _recipeRepository;
        private IIngredientRepository? _ingredientRepository;
        private ICategoryRepository? _categoryRepository;
        private IMealPlanRepository? _mealPlanRepository;
        private IShoppingListRepository? _shoppingListRepository;
        private IUserPantryRepository? _userPantryRepository;
        private IRepository<RecipeCategory>? _recipeCategoryRepository;
        private IRepository<RecipeIngredient>? _recipeIngredientRepository;
        private IRepository<CookingStep>? _cookingStepRepository;
        private IRepository<RecipeNutrition>? _recipeNutritionRepository;
        private IRepository<IngredientNutrition>? _ingredientNutritionRepository;
        private IRepository<MealPlanItem>? _mealPlanItemRepository;
        private IRepository<ShoppingListItem>? _shoppingListItemRepository;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public ApplicationDbContext Context => _context;
        
        public IRecipeRepository Recipes => _recipeRepository ??= new RecipeRepository(_context);
        public IIngredientRepository Ingredients => _ingredientRepository ??= new IngredientRepository(_context);
        public ICategoryRepository Categories => _categoryRepository ??= new CategoryRepository(_context);
        public IMealPlanRepository MealPlans => _mealPlanRepository ??= new MealPlanRepository(_context);
        public IShoppingListRepository ShoppingLists => _shoppingListRepository ??= new ShoppingListRepository(_context);
        public IUserPantryRepository UserPantry => _userPantryRepository ??= new UserPantryRepository(_context);
        
        // Generic repositories for simpler entities
        public IRepository<RecipeCategory> RecipeCategories => 
            _recipeCategoryRepository ??= new Repository<RecipeCategory>(_context);
        
        public IRepository<RecipeIngredient> RecipeIngredients => 
            _recipeIngredientRepository ??= new Repository<RecipeIngredient>(_context);
        
        public IRepository<CookingStep> CookingSteps => 
            _cookingStepRepository ??= new Repository<CookingStep>(_context);
        
        public IRepository<RecipeNutrition> RecipeNutrition => 
            _recipeNutritionRepository ??= new Repository<RecipeNutrition>(_context);
        
        public IRepository<IngredientNutrition> IngredientNutrition => 
            _ingredientNutritionRepository ??= new Repository<IngredientNutrition>(_context);
        
        public IRepository<MealPlanItem> MealPlanItems => 
            _mealPlanItemRepository ??= new Repository<MealPlanItem>(_context);
        
        public IRepository<ShoppingListItem> ShoppingListItems => 
            _shoppingListItemRepository ??= new Repository<ShoppingListItem>(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
        
                if (_transaction != null)
                {
                    await _transaction.CommitAsync();
                }
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync()
        {
            try
            {
                if (_transaction != null)
                {
                    await _transaction.RollbackAsync();
                }
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _transaction?.Dispose();
                    _context.Dispose();
                }
                _disposed = true;
            }
        }
    }
}