// MealPlanner/BLL/Services/Implementations/AppBLL.cs (update constructor)
using AutoMapper;
using MealPlanner.BLL.Services;
using MealPlanner.Data.UnitOfWork;

namespace MealPlanner.BLL.Services.Implementations
{
    /// <summary>
    /// Application BLL implementation
    /// </summary>
    public class AppBLL : IAppBLL
    {
        protected readonly IUnitOfWork UnitOfWork;
        protected readonly IMapper Mapper;
        protected readonly ILoggerFactory LoggerFactory;

        private IRecipeService? _recipeService;
        private IIngredientService? _ingredientService;
        private ICategoryService? _categoryService;
        private IMealPlanService? _mealPlanService;
        private IShoppingListService? _shoppingListService;
        private IUserPantryService? _userPantryService;
        private IRecipeFinderService? _recipeFinderService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork">Unit of work</param>
        /// <param name="mapper">Mapper</param>
        public AppBLL(IUnitOfWork unitOfWork, IMapper mapper, ILoggerFactory loggerFactory)
        {
            UnitOfWork = unitOfWork;
            Mapper = mapper;
            LoggerFactory = loggerFactory;
        }

        /// <summary>
        /// Recipe service
        /// </summary>
        public IRecipeService Recipes => 
            _recipeService ??= new RecipeService(UnitOfWork, Mapper);

        /// <summary>
        /// Ingredient service
        /// </summary>
        public IIngredientService Ingredients => 
            _ingredientService ??= new IngredientService(UnitOfWork, Mapper);

        /// <summary>
        /// Category service
        /// </summary>
        public ICategoryService Categories => 
            _categoryService ??= new CategoryService(UnitOfWork, Mapper);

        /// <summary>
        /// Meal plan service
        /// </summary>
        public IMealPlanService MealPlans => 
            _mealPlanService ??= new MealPlanService(UnitOfWork, Mapper);

        /// <summary>
        /// Shopping list service
        /// </summary>
        public IShoppingListService ShoppingLists => 
            _shoppingListService ??= new ShoppingListService(UnitOfWork, Mapper, LoggerFactory.CreateLogger<ShoppingListService>());

        /// <summary>
        /// User pantry service
        /// </summary>
        public IUserPantryService UserPantry => 
            _userPantryService ??= new UserPantryService(UnitOfWork, Mapper);

        /// <summary>
        /// Recipe finder service
        /// </summary>
        public IRecipeFinderService RecipeFinder => 
            _recipeFinderService ??= new RecipeFinderService(UnitOfWork, Mapper);

        /// <summary>
        /// Save all changes
        /// </summary>
        /// <returns>Number of affected records</returns>
        public async Task<int> SaveChangesAsync()
        {
            return await UnitOfWork.SaveChangesAsync();
        }
    }
}