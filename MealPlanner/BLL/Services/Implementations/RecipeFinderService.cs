// MealPlanner/BLL/Services/Implementations/RecipeFinderService.cs
using AutoMapper;
using MealPlanner.BLL.DTO;
using MealPlanner.Data.UnitOfWork;
using MealPlanner.Data.Repositories;
using MealPlanner.Models.Inventory;
using System.Linq;

namespace MealPlanner.BLL.Services.Implementations
{
    /// <summary>
    /// Recipe finder service implementation
    /// </summary>
    public class RecipeFinderService : IRecipeFinderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork">Unit of work</param>
        /// <param name="mapper">Mapper</param>
        public RecipeFinderService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Find recipes matching ingredients
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="ingredientIds">Available ingredient IDs</param>
        /// <param name="matchAll">Require all ingredients to match</param>
        /// <returns>List of matching recipes with match percentage</returns>
        public async Task<List<DTO.RecipeMatch>> FindRecipesAsync(string userId, List<int> ingredientIds, bool matchAll = false)
        {
            if (ingredientIds == null || !ingredientIds.Any())
            {
                return new List<DTO.RecipeMatch>();
            }

            // Get all user recipes with ingredients
            var userRecipes = await _unitOfWork.Recipes.GetUserRecipesAsync(userId);
            
            var recipesWithIngredients = new List<Models.Inventory.Recipe>();
            foreach (var recipe in userRecipes)
            {
                var recipeWithIngredients = await _unitOfWork.Recipes.GetRecipeWithIngredientsAsync(recipe.Id);
                if (recipeWithIngredients != null)
                {
                    recipesWithIngredients.Add(recipeWithIngredients);
                }
            }

            // Filter recipes based on selected ingredients
            List<Models.Inventory.Recipe> matchingRecipes;
            
            if (matchAll)
            {
                // Find recipes that contain ALL selected ingredients
                matchingRecipes = recipesWithIngredients
                    .Where(r => !ingredientIds
                        .Except(r.RecipeIngredients.Select(ri => ri.IngredientId))
                        .Any())
                    .ToList();
            }
            else
            {
                // Find recipes that contain ANY of the selected ingredients
                matchingRecipes = recipesWithIngredients
                    .Where(r => r.RecipeIngredients
                        .Any(ri => ingredientIds.Contains(ri.IngredientId)))
                    .ToList();
            }

            // Calculate match percentage for each recipe
            var recipeMatches = new List<DTO.RecipeMatch>();
            
            foreach (var recipe in matchingRecipes)
            {
                var recipeIngredientIds = recipe.RecipeIngredients.Select(ri => ri.IngredientId).ToList();
                var matchCount = recipeIngredientIds.Intersect(ingredientIds).Count();
                var matchPercentage = (matchCount * 100) / recipeIngredientIds.Count;

                // Get missing ingredients
                var missingIngredientIds = recipeIngredientIds.Except(ingredientIds).ToList();
                var missingIngredients = await _unitOfWork.Ingredients
                    .ToListAsync(i => missingIngredientIds.Contains(i.Id));

                var recipeMatch = new DTO.RecipeMatch
                {
                    Recipe = _mapper.Map<DTO.Recipe>(recipe),
                    MatchPercentage = matchPercentage,
                    MissingIngredients = missingIngredients.Select(i => _mapper.Map<DTO.Ingredient>(i)).ToList()
                };
                
                recipeMatches.Add(recipeMatch);
            }

            // Order by match percentage (highest first)
            return recipeMatches.OrderByDescending(r => r.MatchPercentage).ToList();
        }

        /// <summary>
        /// Get recipe details with available ingredients highlighted
        /// </summary>
        /// <param name="recipeId">Recipe ID</param>
        /// <param name="availableIngredientIds">Available ingredient IDs</param>
        /// <param name="userId">User ID to verify ownership</param>
        /// <returns>Recipe details with availability</returns>
        public async Task<DTO.RecipeWithAvailability?> GetRecipeWithAvailabilityAsync(int recipeId, List<int> availableIngredientIds, string userId)
        {
            var recipe = await _unitOfWork.Recipes.GetRecipeWithDetailsAsync(recipeId);
            
            if (recipe == null)
            {
                return null;
            }
            
            // Verify ownership if userId provided
            if (userId != null && recipe.UserId != userId)
            {
                return null;
            }
            
            // Map to BLL DTO
            var recipeDetail = _mapper.Map<DTO.RecipeDetail>(recipe);
            
            // Create recipe ingredients with availability flag
            var recipeIngredients = new List<DTO.RecipeIngredient>();
            
            foreach (var ingredient in recipe.RecipeIngredients)
            {
                var recipeIngredient = _mapper.Map<DTO.RecipeIngredient>(ingredient);
                recipeIngredient.IsAvailable = availableIngredientIds?.Contains(ingredient.IngredientId) ?? false;
                recipeIngredients.Add(recipeIngredient);
            }
            
            // Create result
            var result = new DTO.RecipeWithAvailability
            {
                Recipe = recipeDetail,
                RecipeIngredients = recipeIngredients
            };
            
            return result;
        }
    }
}