// MealPlanner.BLL/Services/Implementations/RecipeService.cs

using AutoMapper;
using MealPlanner.BLL.DTO;
using MealPlanner.Data.UnitOfWork;
using MealPlanner.Data.Repositories;
using MealPlanner.Models.Inventory;
using System.Linq;
using MealPlanner.Data;
using MealPlanner.Dto;
using MealPlanner.Models.Inventory.ViewModels.Recipes;
using MealPlanner.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace MealPlanner.BLL.Services.Implementations
{
    /// <summary>
    /// Recipe service implementation
    /// </summary>
    public class RecipeService : BaseService<DTO.Recipe, Models.Inventory.Recipe, int>, IRecipeService
    {
        /// <summary>
        /// Recipe repository
        /// </summary>
        private readonly IRecipeRepository _recipeRepository;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork">Unit of work</param>
        /// <param name="mapper">Mapper</param>
        public RecipeService(IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper, unitOfWork.Recipes)
        {
            _recipeRepository = unitOfWork.Recipes;
        }

        /// <summary>
        /// Apply user filter to query if needed
        /// </summary>
        /// <param name="query">Query to filter</param>
        /// <param name="userId">User ID</param>
        /// <returns>Filtered query</returns>
        protected override IQueryable<Models.Inventory.Recipe> ApplyUserFilter(
            IQueryable<Models.Inventory.Recipe> query, string? userId)
        {
            if (userId != null)
            {
                query = query.Where(r => r.UserId == userId);
            }

            return query;
        }

        /// <summary>
        /// Verify if user owns this entity
        /// </summary>
        /// <param name="entity">Entity to check</param>
        /// <param name="userId">User ID</param>
        /// <returns>True if user owns this entity</returns>
        protected override bool VerifyOwnership(Models.Inventory.Recipe entity, string userId)
        {
            return entity.UserId == userId;
        }

        /// <summary>
        /// Set owner of entity
        /// </summary>
        /// <param name="entity">Entity to set owner</param>
        /// <param name="userId">User ID</param>
        protected override void SetOwner(Models.Inventory.Recipe entity, string userId)
        {
            entity.UserId = userId;
            entity.CreatedAt = DateTime.UtcNow; // Set creation date
        }

        public override async Task<List<BLL.DTO.Recipe>> AllAsync(string? userId = null)
        {
            List<Models.Inventory.Recipe> recipes;

            if (userId == null)
            {
                recipes = await _recipeRepository.GetAllAsync();
            }
            else
            {
                recipes = await _recipeRepository.GetUserRecipesAsync(userId);
            }

            return recipes.Select(r => Mapper.Map<BLL.DTO.Recipe>(r)).ToList();
        }


        /// <summary>
        /// Get recipe with all related details
        /// </summary>
        /// <param name="id">Recipe ID</param>
        /// <param name="userId">User ID to verify ownership</param>
        /// <returns>Recipe with details or null</returns>
        public async Task<DTO.RecipeDetail?> GetWithDetailsAsync(int id, string? userId = null)
        {
            var recipe = await _recipeRepository.GetRecipeWithDetailsAsync(id);

            if (recipe == null)
            {
                return null;
            }

            // Verify ownership if userId provided
            if (userId != null && recipe.UserId != userId)
            {
                return null;
            }

            return Mapper.Map<DTO.RecipeDetail>(recipe);
        }

        /// <summary>
        /// Get favorite recipes for user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of favorite recipes</returns>
        public async Task<List<DTO.Recipe>> GetFavoritesAsync(string userId)
        {
            var favorites = await _recipeRepository.GetFavoriteRecipesAsync(userId);
            return favorites.Select(r => Mapper.Map<DTO.Recipe>(r)).ToList();
        }

        /// <summary>
        /// Get recent recipes for user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="count">Number of recipes to return</param>
        /// <returns>List of recent recipes</returns>
        public async Task<List<DTO.Recipe>> GetRecentAsync(string userId, int count = 5)
        {
            var recent = await _recipeRepository.GetRecentRecipesAsync(userId, count);
            return recent.Select(r => Mapper.Map<DTO.Recipe>(r)).ToList();
        }

        // Implement the cooking steps methods in your RecipeService class
        public async Task<BLL.DTO.CookingStep?> GetCookingStepAsync(int id, string userId)
        {
            var cookingStep = await UnitOfWork.CookingSteps.GetByIdAsync(id);
            if (cookingStep == null)
            {
                return null;
            }

            // Verify ownership
            var recipe = await UnitOfWork.Recipes.GetByIdAsync(cookingStep.RecipeId);
            if (recipe == null || recipe.UserId != userId)
            {
                return null;
            }

            return Mapper.Map<BLL.DTO.CookingStep>(cookingStep);
        }

        public async Task<BLL.DTO.CookingStep> AddCookingStepAsync(BLL.DTO.CookingStep step, string userId)
        {
            // Verify recipe ownership
            var recipe = await UnitOfWork.Recipes.GetByIdAsync(step.RecipeId);
            if (recipe == null || recipe.UserId != userId)
            {
                throw new UnauthorizedAccessException($"User {userId} does not own recipe with ID {step.RecipeId}");
            }

            // Map to domain model
            var cookingStep = Mapper.Map<Models.Inventory.CookingStep>(step);

            // Add to repository
            UnitOfWork.CookingSteps.Add(cookingStep);
            await UnitOfWork.SaveChangesAsync();

            return Mapper.Map<BLL.DTO.CookingStep>(cookingStep);
        }

        public async Task<BLL.DTO.CookingStep> UpdateCookingStepAsync(BLL.DTO.CookingStep step, string userId)
        {
            // Get cooking step to verify it exists
            var cookingStep = await UnitOfWork.CookingSteps.GetByIdAsync(step.Id);
            if (cookingStep == null)
            {
                throw new ArgumentException($"Cooking step with ID {step.Id} not found");
            }

            // Verify recipe ownership
            var recipe = await UnitOfWork.Recipes.GetByIdAsync(cookingStep.RecipeId);
            if (recipe == null || recipe.UserId != userId)
            {
                throw new UnauthorizedAccessException(
                    $"User {userId} does not own recipe with ID {cookingStep.RecipeId}");
            }

            // Update the entity
            Mapper.Map(step, cookingStep);
            UnitOfWork.CookingSteps.Update(cookingStep);
            await UnitOfWork.SaveChangesAsync();

            return Mapper.Map<BLL.DTO.CookingStep>(cookingStep);
        }

        public async Task<bool> RemoveCookingStepAsync(int id, string userId)
        {
            // Get cooking step
            var cookingStep = await UnitOfWork.CookingSteps.GetByIdAsync(id);
            if (cookingStep == null)
            {
                return false;
            }

            // Verify recipe ownership
            var recipe = await UnitOfWork.Recipes.GetByIdAsync(cookingStep.RecipeId);
            if (recipe == null || recipe.UserId != userId)
            {
                throw new UnauthorizedAccessException(
                    $"User {userId} does not own recipe with ID {cookingStep.RecipeId}");
            }

            // Remove the step
            UnitOfWork.CookingSteps.Remove(cookingStep);
            await UnitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task ReorderCookingStepsAsync(int recipeId, string userId)
        {
            // Verify recipe ownership
            var recipe = await UnitOfWork.Recipes.GetByIdAsync(recipeId);
            if (recipe == null || recipe.UserId != userId)
            {
                throw new UnauthorizedAccessException($"User {userId} does not own recipe with ID {recipeId}");
            }

            // Get all cooking steps for this recipe
            var steps = await UnitOfWork.CookingSteps
                .ToListAsync(cs => cs.RecipeId == recipeId);

            // Sort by step number
            steps = steps.OrderBy(cs => cs.StepNumber).ToList();

            // Reassign step numbers
            for (int i = 0; i < steps.Count; i++)
            {
                steps[i].StepNumber = i + 1;
                UnitOfWork.CookingSteps.Update(steps[i]);
            }

            await UnitOfWork.SaveChangesAsync();
        }

        public async Task ReorderCookingStepsAsync(int recipeId, int[] stepIds, string userId)
        {
            // Verify recipe ownership
            var recipe = await UnitOfWork.Recipes.GetByIdAsync(recipeId);
            if (recipe == null || recipe.UserId != userId)
            {
                throw new UnauthorizedAccessException($"User {userId} does not own recipe with ID {recipeId}");
            }

            // Reorder the steps based on the provided IDs
            for (int i = 0; i < stepIds.Length; i++)
            {
                var step = await UnitOfWork.CookingSteps.GetByIdAsync(stepIds[i]);
                if (step != null && step.RecipeId == recipeId)
                {
                    step.StepNumber = i + 1;
                    UnitOfWork.CookingSteps.Update(step);
                }
            }

            await UnitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Update or create recipe nutrition information
        /// </summary>
        /// <param name="nutrition">Recipe nutrition data</param>
        /// <param name="userId">User ID to verify ownership</param>
        /// <returns>Updated recipe nutrition</returns>
        public async Task<BLL.DTO.RecipeNutrition> UpdateNutritionAsync(BLL.DTO.RecipeNutrition nutrition,
            string userId)
        {
            // Get recipe to check ownership
            var recipe = await _recipeRepository.GetByIdAsync(nutrition.RecipeId);
            if (recipe == null)
            {
                throw new ArgumentException($"Recipe with ID {nutrition.RecipeId} not found");
            }

            if (recipe.UserId != userId)
            {
                throw new UnauthorizedAccessException(
                    $"User {userId} does not own recipe with ID {nutrition.RecipeId}");
            }

            // Check if nutrition exists
            var existingNutrition =
                await UnitOfWork.RecipeNutrition.SingleOrDefaultAsync(n => n.RecipeId == nutrition.RecipeId);

            if (existingNutrition == null)
            {
                // Create new nutrition
                var newNutrition = Mapper.Map<Models.Inventory.RecipeNutrition>(nutrition);
                UnitOfWork.RecipeNutrition.Add(newNutrition);
                await UnitOfWork.SaveChangesAsync();

                return Mapper.Map<BLL.DTO.RecipeNutrition>(newNutrition);
            }
            else
            {
                // Update existing nutrition
                Mapper.Map(nutrition, existingNutrition);
                UnitOfWork.RecipeNutrition.Update(existingNutrition);
                await UnitOfWork.SaveChangesAsync();

                return Mapper.Map<BLL.DTO.RecipeNutrition>(existingNutrition);
            }
        }

        /// <summary>
        /// Calculate recipe nutrition based on ingredients
        /// </summary>
        /// <param name="recipeId">Recipe ID</param>
        /// <param name="userId">User ID to verify ownership</param>
        /// <returns>Calculated recipe nutrition</returns>
        public async Task<BLL.DTO.RecipeNutrition> CalculateNutritionAsync(int recipeId, string userId)
        {
            // Get recipe with ingredients
            var recipe = await _recipeRepository.GetRecipeWithIngredientsAsync(recipeId);
            if (recipe == null)
            {
                throw new ArgumentException($"Recipe with ID {recipeId} not found");
            }

            if (recipe.UserId != userId)
            {
                throw new UnauthorizedAccessException($"User {userId} does not own recipe with ID {recipeId}");
            }

            // Calculate nutrition totals
            decimal totalCalories = 0;
            decimal totalProtein = 0;
            decimal totalCarbs = 0;
            decimal totalFat = 0;
            decimal totalFiber = 0;
            decimal totalSugar = 0;
            decimal totalSodium = 0;

            foreach (var recipeIngredient in recipe.RecipeIngredients)
            {
                var ingredient =
                    await UnitOfWork.Ingredients.GetIngredientWithNutritionAsync(recipeIngredient.IngredientId);
                if (ingredient?.IngredientNutrition != null)
                {
                    // Multiplier based on amount (assuming nutrition is per 100g)
                    decimal multiplier = recipeIngredient.Amount / 100;

                    totalCalories += ingredient.IngredientNutrition.CaloriesPer100g * multiplier;
                    totalProtein += ingredient.IngredientNutrition.ProteinG * multiplier;
                    totalCarbs += ingredient.IngredientNutrition.CarbsG * multiplier;
                    totalFat += ingredient.IngredientNutrition.FatG * multiplier;
                    totalFiber += ingredient.IngredientNutrition.FiberG * multiplier;
                    totalSugar += ingredient.IngredientNutrition.SugarG * multiplier;
                    totalSodium += ingredient.IngredientNutrition.SodiumMg * multiplier / 1000; // Convert mg to g
                }
            }

            // Get or create recipe nutrition
            var recipeNutrition = await UnitOfWork.RecipeNutrition.SingleOrDefaultAsync(rn => rn.RecipeId == recipeId);
            if (recipeNutrition == null)
            {
                // Create new nutrition entry
                recipeNutrition = new Models.Inventory.RecipeNutrition
                {
                    RecipeId = recipeId
                };
                UnitOfWork.RecipeNutrition.Add(recipeNutrition);
            }

            // Update nutrition values
            recipeNutrition.TotalCalories = totalCalories;
            recipeNutrition.TotalProteinG = totalProtein;
            recipeNutrition.TotalCarbsG = totalCarbs;
            recipeNutrition.TotalFatG = totalFat;
            recipeNutrition.TotalFiberG = totalFiber;
            recipeNutrition.TotalSugarG = totalSugar;
            recipeNutrition.TotalSodiumG = totalSodium;

            // Calculate calories per serving
            if (recipe.DefaultServings > 0)
            {
                recipeNutrition.CaloriesPerServing = totalCalories / recipe.DefaultServings;
            }
            else
            {
                recipeNutrition.CaloriesPerServing = totalCalories;
            }

            // Save changes
            UnitOfWork.RecipeNutrition.Update(recipeNutrition);
            await UnitOfWork.SaveChangesAsync();

            return Mapper.Map<BLL.DTO.RecipeNutrition>(recipeNutrition);
        }

        public async Task<DTO.Recipe> AddFullRecipeAsync(
            RecipeCreateDto recipeDto,
            string userId,
            int[] categoryIds,
            List<(int ingredientId, decimal amount, string unit)> ingredients)
        {
            // 🔐 SECURE: Use execution strategy for retry logic and transaction safety
            var strategy = UnitOfWork.Context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                // 🔐 SECURE: All operations in a single transaction
                await UnitOfWork.BeginTransactionAsync();

                try
                {
                    // Create recipe
                    var recipe = Mapper.Map<Models.Inventory.Recipe>(recipeDto);
                    SetOwner(recipe, userId);
                    if (string.IsNullOrEmpty(recipe.ImageUrl))
                    {
                        recipe.ImageUrl = string.Empty; // or provide a default value
                    }
                    
                    UnitOfWork.Recipes.Add(recipe);
                    await UnitOfWork.SaveChangesAsync();

                    // Add categories - all or nothing
                    foreach (var categoryId in categoryIds)
                    {
                        UnitOfWork.RecipeCategories.Add(new Models.Inventory.RecipeCategory
                        {
                            RecipeId = recipe.Id,
                            CategoryId = categoryId
                        });
                    }

                    // Add ingredients - all or nothing
                    foreach (var (ingredientId, amount, unit) in ingredients)
                    {
                        UnitOfWork.RecipeIngredients.Add(new Models.Inventory.RecipeIngredient
                        {
                            RecipeId = recipe.Id,
                            IngredientId = ingredientId,
                            Amount = amount,
                            Unit = unit ?? string.Empty
                        });
                    }

                    await UnitOfWork.SaveChangesAsync();

                    // Calculate nutrition (this might fail, but won't corrupt data)
                    try
                    {
                        await CalculateNutritionAsync(recipe.Id, userId);
                    }
                    catch (Exception ex)
                    {
                        // Log but don't fail the entire operation
                        // Nutrition can be calculated later
                        Console.WriteLine($"Warning: Could not calculate nutrition: {ex.Message}");
                    }

                    // 🔐 SECURE: Commit all changes atomically
                    await UnitOfWork.CommitTransactionAsync();

                    return Mapper.Map<DTO.Recipe>(recipe);
                }
                catch (Exception)
                {
                    // 🔐 SECURE: Rollback on any failure
                    await UnitOfWork.RollbackTransactionAsync();
                    throw; // Re-throw to let caller handle
                }
            });
        }


        public async Task<DTO.Recipe> UpdateFullRecipeAsync(
            RecipeUpdateDto recipeDto,
            string userId,
            List<int> categoryIds,
            Dictionary<int, RecipeIngredientInfo> ingredients)
        {
            await UnitOfWork.BeginTransactionAsync();
            try
            {
                var recipe = await UnitOfWork.Recipes.GetByIdAsync(recipeDto.Id);
                if (recipe == null || recipe.UserId != userId)
                    throw new UnauthorizedAccessException("You do not own this recipe.");

                Mapper.Map(recipeDto, recipe);
                UnitOfWork.Recipes.Update(recipe);
                await UnitOfWork.SaveChangesAsync();

                // Replace all existing categories
                var existingCats = await UnitOfWork.RecipeCategories.ToListAsync(rc => rc.RecipeId == recipe.Id);
                UnitOfWork.RecipeCategories.RemoveRange(existingCats);

                foreach (var categoryId in categoryIds)
                {
                    UnitOfWork.RecipeCategories.Add(new Models.Inventory.RecipeCategory
                    {
                        RecipeId = recipe.Id,
                        CategoryId = categoryId
                    });
                }

                // Replace all existing ingredients
                var existingIngs = await UnitOfWork.RecipeIngredients.ToListAsync(ri => ri.RecipeId == recipe.Id);
                UnitOfWork.RecipeIngredients.RemoveRange(existingIngs);

                foreach (var pair in ingredients)
                {
                    var ingredientId = pair.Key;
                    var info = pair.Value;

                    UnitOfWork.RecipeIngredients.Add(new Models.Inventory.RecipeIngredient
                    {
                        RecipeId = recipe.Id,
                        IngredientId = ingredientId,
                        Amount = info.Amount,
                        Unit = info.Unit ?? string.Empty
                    });
                }

                await UnitOfWork.SaveChangesAsync();

                await CalculateNutritionAsync(recipe.Id, userId);

                await UnitOfWork.CommitTransactionAsync();
                return Mapper.Map<DTO.Recipe>(recipe);
            }
            catch
            {
                await UnitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
}