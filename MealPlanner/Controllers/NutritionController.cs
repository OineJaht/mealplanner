using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using MealPlanner.BLL.Services;

using MealPlanner.Dto;
using MealPlanner.Models.Inventory;
using MealPlanner.Models.ViewModels;
using MealPlanner.Models.ViewModels.Nutrition;

namespace MealPlanner.Controllers
{
    [Authorize(AuthenticationSchemes = "AuthScheme")]
    public class NutritionController : Controller
    {
        private readonly IAppBLL _bll;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<NutritionController> _logger;
        private readonly IMapper _mapper;

        public NutritionController(
            IAppBLL bll,
            UserManager<ApplicationUser> userManager,
            ILogger<NutritionController> logger,
            IMapper mapper)
        {
            _bll = bll;
            _userManager = userManager;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: Nutrition/RecipeNutrition/5
        public async Task<IActionResult> RecipeNutrition(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            // Get recipe with details using BLL service
            var recipe = await _bll.Recipes.GetWithDetailsAsync(id.Value, currentUser.Id);
            
            if (recipe == null)
            {
                return NotFound();
            }

            // Create view model
            var viewModel = new RecipeNutritionViewModel
            {
                RecipeId = recipe.Id,
                RecipeName = recipe.Name,
                DefaultServings = recipe.DefaultServings
            };

            // If nutrition info doesn't exist, create a default one
            if (recipe.Nutrition == null)
            {
                viewModel.Nutrition = new RecipeNutritionDto
                {
                    RecipeId = recipe.Id,
                    TotalCalories = 0,
                    TotalProteinG = 0,
                    TotalCarbsG = 0,
                    TotalFatG = 0,
                    TotalFiberG = 0,
                    TotalSugarG = 0,
                    TotalSodiumG = 0,
                    CaloriesPerServing = 0
                };
            }
            else
            {
                viewModel.Nutrition = _mapper.Map<RecipeNutritionDto>(recipe.Nutrition);
                // Ensure RecipeId is set
                viewModel.Nutrition.RecipeId = recipe.Id;
            }

            return View(viewModel);
        }

        // POST: Nutrition/UpdateRecipeNutrition/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRecipeNutrition(int id, RecipeNutritionViewModel viewModel)
        {
            if (id != viewModel.Nutrition.Id)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            // Get recipe to check ownership
            var recipe = await _bll.Recipes.FindAsync(viewModel.Nutrition.RecipeId, currentUser.Id);
            if (recipe == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Update recipe nutrition using BLL service
                    await _bll.SaveChangesAsync();
                    
                    _logger.LogInformation("Recipe nutrition updated successfully for recipe ID {RecipeId}", viewModel.Nutrition.RecipeId);
                    
                    return RedirectToAction("Details", "Recipes", new { id = viewModel.Nutrition.RecipeId });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating recipe nutrition: {Message}", ex.Message);
                    ModelState.AddModelError("", "An error occurred while updating recipe nutrition: " + ex.Message);
                }
            }

            // Repopulate view model if we get here
            viewModel.RecipeId = recipe.Id;
            viewModel.RecipeName = recipe.Name;
            viewModel.DefaultServings = recipe.DefaultServings;

            return View("RecipeNutrition", viewModel);
        }

        // GET: Nutrition/CalculateRecipeNutrition/5
        public async Task<IActionResult> CalculateRecipeNutrition(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            try
            {
                // Calculate nutrition using BLL service
                await _bll.Recipes.CalculateNutritionAsync(id, currentUser.Id);
                await _bll.SaveChangesAsync();
                
                _logger.LogInformation("Successfully calculated nutrition for recipe ID {RecipeId}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating recipe nutrition: {Message}", ex.Message);
                // Continue to redirect even if there was an error
            }

            return RedirectToAction("Details", "Recipes", new { id });
        }

        // GET: Nutrition/IngredientNutrition/5
        public async Task<IActionResult> IngredientNutrition(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Get ingredient with nutrition using BLL service
            var ingredient = await _bll.Ingredients.GetWithNutritionAsync(id.Value);
    
            if (ingredient == null)
            {
                return NotFound();
            }

            // Create view model
            var viewModel = new IngredientNutritionViewModel
            {
                IngredientId = ingredient.Id,
                IngredientName = ingredient.Name,
                IngredientCategory = ingredient.Category
            };

            // ✅ FIX: Proper handling of existing vs new nutrition
            if (ingredient.Nutrition != null)
            {
                // Nutrition exists - map it properly
                viewModel.Nutrition = _mapper.Map<IngredientNutritionDto>(ingredient.Nutrition);
                // Ensure IDs are set correctly
                viewModel.Nutrition.Id = ingredient.Nutrition.Id;
                viewModel.Nutrition.IngredientId = ingredient.Id;
        
                _logger.LogInformation(
                    "Loaded existing nutrition - ID: {NutritionId}, Calories: {Calories}, Protein: {Protein}",
                    ingredient.Nutrition.Id, ingredient.Nutrition.CaloriesPer100g, ingredient.Nutrition.ProteinG);
            }
            else
            {
                // No nutrition exists - create empty one for CREATE form
                viewModel.Nutrition = new IngredientNutritionDto
                {
                    Id = 0, // This triggers CREATE form in view
                    IngredientId = ingredient.Id,
                    CaloriesPer100g = 0,
                    ProteinG = 0,
                    CarbsG = 0,
                    FatG = 0,
                    FiberG = 0,
                    SugarG = 0,
                    SodiumMg = 0
                };
        
                _logger.LogInformation("No nutrition found for ingredient {IngredientId} - showing CREATE form", ingredient.Id);
            }

            return View(viewModel);
        }
        
        // POST: Nutrition/UpdateIngredientNutrition
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> UpdateIngredientNutrition(int id, IngredientNutritionViewModel viewModel)
{
    _logger.LogInformation(
        "UpdateIngredientNutrition called for ID: {Id}, IngredientId: {IngredientId}",
        id, viewModel.Nutrition.IngredientId);

    if (id != viewModel.Nutrition.Id)
    {
        _logger.LogWarning("ID mismatch: {Id} != {DtoId}", id, viewModel.Nutrition.Id);
        return NotFound();
    }
    
    var currentUser = await _userManager.GetUserAsync(User);
    if (currentUser == null)
    {
        return Challenge();
    }

    if (!ModelState.IsValid)
    {
        _logger.LogWarning("Invalid model state when updating ingredient nutrition");
        foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
        {
            _logger.LogWarning("Model error: {ErrorMessage}", error.ErrorMessage);
        }
        
        // Repopulate view model
        var ingredient = await _bll.Ingredients.FindAsync(viewModel.IngredientId);
        if (ingredient != null)
        {
            viewModel.IngredientName = ingredient.Name;
            viewModel.IngredientCategory = ingredient.Category;
        }
        
        return View("IngredientNutrition", viewModel);
    }

    try
    {
        // ✅ FIX: Actually call the BLL method to update nutrition
        var bllNutrition = _mapper.Map<BLL.DTO.IngredientNutrition>(viewModel.Nutrition);
        
        await _bll.Ingredients.UpdateNutritionAsync(bllNutrition, currentUser.Id);
        await _bll.SaveChangesAsync();
        
        _logger.LogInformation("Successfully saved nutrition for IngredientId: {IngredientId}", viewModel.Nutrition.IngredientId);
        
        return RedirectToAction("Details", "Ingredients", new { id = viewModel.Nutrition.IngredientId });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error saving nutrition: {Message}", ex.Message);
        
        // Log any model state errors
        foreach (var modelState in ModelState.Values)
        {
            foreach (var error in modelState.Errors)
            {
                _logger.LogWarning("ModelState error: {ErrorMessage}", error.ErrorMessage);
            }
        }
        
        // Repopulate view model
        var ingredient = await _bll.Ingredients.FindAsync(viewModel.IngredientId);
        if (ingredient != null)
        {
            viewModel.IngredientName = ingredient.Name;
            viewModel.IngredientCategory = ingredient.Category;
        }
        
        return View("IngredientNutrition", viewModel);
    }
}

        // POST: Nutrition/CreateIngredientNutrition
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> CreateIngredientNutrition(int id, IngredientNutritionCreateDto nutritionDto)
{
    _logger.LogInformation("Creating nutrition for ingredient ID: {Id}", id);
    _logger.LogInformation(
        "Received nutrition data: Calories={Calories}, Protein={Protein}",
        nutritionDto.CaloriesPer100g, nutritionDto.ProteinG);

    var currentUser = await _userManager.GetUserAsync(User);
    if (currentUser == null)
    {
        return Challenge();
    }

    // Verify the ingredient exists using BLL service
    var ingredient = await _bll.Ingredients.FindAsync(id);
    if (ingredient == null)
    {
        _logger.LogWarning("Ingredient {Id} not found", id);
        return NotFound();
    }

    _logger.LogInformation("Found ingredient: {Name}", ingredient.Name);

    // Check if nutrition already exists
    var existingIngredient = await _bll.Ingredients.GetWithNutritionAsync(id);
    var existingNutrition = existingIngredient?.Nutrition;

    if (existingNutrition != null)
    {
        _logger.LogWarning("Nutrition already exists for ingredient {Id}", id);
        return RedirectToAction("IngredientNutrition", new { id });
    }

    try {
        // ✅ FIX: Actually call the BLL method to add nutrition
        var bllNutrition = _mapper.Map<BLL.DTO.IngredientNutrition>(nutritionDto);
        bllNutrition.IngredientId = id; // Make sure IngredientId is set
        
        await _bll.Ingredients.AddNutritionAsync(id, bllNutrition, currentUser.Id);
        await _bll.SaveChangesAsync();
        
        _logger.LogInformation("Nutrition saved successfully for ingredient ID: {Id}", id);
        
        return RedirectToAction("IngredientNutrition", new { id });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error saving nutrition data");
        ModelState.AddModelError("", "Error saving nutrition data: " + ex.Message);

        // Create view model for error display
        var viewModel = new IngredientNutritionViewModel
        {
            IngredientId = id,
            IngredientName = ingredient.Name,
            IngredientCategory = ingredient.Category,
            Nutrition = _mapper.Map<IngredientNutritionDto>(nutritionDto)
        };
        viewModel.Nutrition.IngredientId = id;
        
        return View("IngredientNutrition", viewModel);
    }
}

        // GET: Nutrition/NutritionSummary
        [Authorize(AuthenticationSchemes = "AuthScheme")]
        public async Task<IActionResult> NutritionSummary()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            // Get all meal plans with nutrition information using BLL service
            var mealPlanSummary = await _bll.MealPlans.GetNutritionSummaryAsync(currentUser.Id);
            
            // Create view model
            var viewModel = new NutritionSummaryViewModel
            {
                MealPlans = _mapper.Map<List<MealPlanDetailDto>>(mealPlanSummary)
            };

            return View(viewModel);
        }
    }
}