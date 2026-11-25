// MealPlanner/BLL/Services/Implementations/IngredientService.cs
using AutoMapper;
// using MealPlanner.BLL.DTO;
using MealPlanner.Data.UnitOfWork;
using MealPlanner.Data.Repositories;

using System.Linq;
using MealPlanner.Models.Inventory;

namespace MealPlanner.BLL.Services.Implementations
{
    /// <summary>
    /// Ingredient service implementation
    /// </summary>
    public class IngredientService : BaseService<DTO.Ingredient, Ingredient, int>, IIngredientService
    {
        /// <summary>
        /// Ingredient repository
        /// </summary>
        private readonly IIngredientRepository _ingredientRepository;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork">Unit of work</param>
        /// <param name="mapper">Mapper</param>
        public IngredientService(IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper, unitOfWork.Ingredients)
        {
            _ingredientRepository = unitOfWork.Ingredients;
        }
        
        public async Task<List<DTO.Ingredient>> GetAllAsync()
        {
            var ingredients = await _ingredientRepository.GetAllAsync();
            return ingredients.Select(i => Mapper.Map<DTO.Ingredient>(i)).ToList();
        }



        /// <summary>
        /// Get ingredient with nutrition information
        /// </summary>
        /// <param name="id">Ingredient ID</param>
        /// <returns>Ingredient with nutrition or null</returns>
        public async Task<DTO.IngredientDetail?> GetWithNutritionAsync(int id)
        {
            var ingredient = await _ingredientRepository.GetIngredientWithNutritionAsync(id);
            
            if (ingredient == null)
            {
                return null;
            }
            
            return Mapper.Map<DTO.IngredientDetail>(ingredient);
        }

        /// <summary>
        /// Search ingredients by name or category
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <returns>List of matching ingredients</returns>
        public async Task<List<DTO.Ingredient>> SearchAsync(string searchTerm)
        {
            var ingredients = await _ingredientRepository.SearchIngredientsAsync(searchTerm);
            return ingredients.Select(i => Mapper.Map<DTO.Ingredient>(i)).ToList();
        }

        /// <summary>
        /// Get ingredients by category
        /// </summary>
        /// <param name="category">Category name</param>
        /// <returns>List of ingredients in category</returns>
        public async Task<List<DTO.Ingredient>> GetByCategoryAsync(string category)
        {
            var ingredients = await _ingredientRepository.GetIngredientsByCategoryAsync(category);
            return ingredients.Select(i => Mapper.Map<DTO.Ingredient>(i)).ToList();
        }
        
        /// <summary>
        /// Add nutrition information to an ingredient
        /// </summary>
        /// <param name="ingredientId">Ingredient ID</param>
        /// <param name="nutrition">Nutrition data</param>
        /// <param name="userId">User ID</param>
        /// <returns>Created ingredient nutrition</returns>
        public async Task<BLL.DTO.IngredientNutrition> AddNutritionAsync(int ingredientId, BLL.DTO.IngredientNutrition nutrition, string userId)
        {
            // Check if ingredient exists
            var ingredient = await _ingredientRepository.GetByIdAsync(ingredientId);
            if (ingredient == null)
            {
                throw new ArgumentException($"Ingredient with ID {ingredientId} not found");
            }
    
            // Check if nutrition already exists
            var existingIngredient = await _ingredientRepository.GetIngredientWithNutritionAsync(ingredientId);
            if (existingIngredient?.IngredientNutrition != null)
            {
                throw new InvalidOperationException($"Nutrition information already exists for ingredient with ID {ingredientId}");
            }
    
            // Create new nutrition
            var newNutrition = Mapper.Map<IngredientNutrition>(nutrition);
            newNutrition.IngredientId = ingredientId;
    
            UnitOfWork.IngredientNutrition.Add(newNutrition);
            await UnitOfWork.SaveChangesAsync();
    
            return Mapper.Map<BLL.DTO.IngredientNutrition>(newNutrition);
        }
        
        /// <summary>
        /// Update ingredient nutrition information
        /// </summary>
        /// <param name="nutrition">Nutrition data</param>
        /// <param name="userId">User ID</param>
        /// <returns>Updated ingredient nutrition</returns>
        public async Task<BLL.DTO.IngredientNutrition> UpdateNutritionAsync(BLL.DTO.IngredientNutrition nutrition, string userId)
        {
            // Check if nutrition exists
            var existingNutrition = await UnitOfWork.IngredientNutrition.GetByIdAsync(nutrition.Id);
            if (existingNutrition == null)
            {
                throw new ArgumentException($"Nutrition with ID {nutrition.Id} not found");
            }
    
            // Verify the ingredient exists
            var ingredient = await _ingredientRepository.GetByIdAsync(existingNutrition.IngredientId);
            if (ingredient == null)
            {
                throw new ArgumentException($"Ingredient with ID {existingNutrition.IngredientId} not found");
            }
    
            // Update nutrition
            Mapper.Map(nutrition, existingNutrition);
            UnitOfWork.IngredientNutrition.Update(existingNutrition);
            await UnitOfWork.SaveChangesAsync();
    
            return Mapper.Map<BLL.DTO.IngredientNutrition>(existingNutrition);
        }
        
        
    }
}