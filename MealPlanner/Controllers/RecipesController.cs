using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using AutoMapper;
using MealPlanner.BLL.Services;
using MealPlanner.Data.UnitOfWork;
using MealPlanner.Dto;
using MealPlanner.Models.Inventory;
using MealPlanner.Models.Inventory.ViewModels.Recipes;
using MealPlanner.Models.ViewModels;

namespace MealPlanner.Controllers
{
    [Authorize(AuthenticationSchemes = "AuthScheme")]
    public class RecipesController : Controller
    {
        private readonly IAppBLL _bll;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RecipesController> _logger;
        private readonly IMapper _mapper;

        public RecipesController(
            IAppBLL bll,
            UserManager<ApplicationUser> userManager,
            ILogger<RecipesController> logger,
            IMapper mapper)
        {
            _bll = bll;
            _userManager = userManager;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: Recipes
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            // Use BLL service instead of repository directly
            var recipes = await _bll.Recipes.AllAsync(currentUser.Id);
            var recipeDtos = _mapper.Map<List<RecipeDto>>(recipes);

            return View(recipeDtos);
        }

        // GET: Recipes/Details/5
        public async Task<IActionResult> Details(int? id)
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

            // Use BLL service to get recipe with details
            var recipe = await _bll.Recipes.GetWithDetailsAsync(id.Value, currentUser.Id);

            if (recipe == null)
            {
                return NotFound();
            }

            var recipeDetailDto = _mapper.Map<RecipeDetailDto>(recipe);
            return View(recipeDetailDto);
        }

        // GET: Recipes/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = new RecipeCreateViewModel
            {
                Recipe = new RecipeCreateDto
                {
                    CreatedAt = DateTime.Now,
                    DefaultServings = 4 // Default value
                }
            };

            await RepopulateCreateViewModelAsync(viewModel);
            return View(viewModel);
        }


        // POST: Recipes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RecipeCreateViewModel viewModel)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            // 🔍 DEBUG: Log what we're receiving
            _logger.LogInformation("=== DEBUGGING FORM SUBMISSION ===");
            _logger.LogInformation($"Recipe Name: {viewModel.Recipe?.Name}");
            _logger.LogInformation($"SelectedCategoryIds Count: {viewModel.SelectedCategoryIds?.Count ?? 0}");
            _logger.LogInformation($"SelectedIngredientIds Count: {viewModel.SelectedIngredientIds?.Count ?? 0}");
            _logger.LogInformation($"IngredientAmounts Count: {viewModel.IngredientAmounts?.Count ?? 0}");
            _logger.LogInformation($"IngredientUnits Count: {viewModel.IngredientUnits?.Count ?? 0}");

            if (viewModel.SelectedCategoryIds != null)
            {
                _logger.LogInformation($"Categories: [{string.Join(", ", viewModel.SelectedCategoryIds)}]");
            }

            if (viewModel.SelectedIngredientIds != null)
            {
                _logger.LogInformation($"Ingredients: [{string.Join(", ", viewModel.SelectedIngredientIds)}]");
            }

            // 🔍 DEBUG: Log raw form data
            _logger.LogInformation("=== RAW FORM DATA ===");
            foreach (var key in Request.Form.Keys)
            {
                var values = Request.Form[key];
                _logger.LogInformation($"{key}: [{string.Join(", ", values)}]");
            }

            // 🔍 DEBUG: Log ModelState errors
            _logger.LogInformation("=== MODEL STATE ERRORS ===");
            foreach (var modelState in ModelState)
            {
                if (modelState.Value.Errors.Count > 0)
                {
                    _logger.LogInformation(
                        $"{modelState.Key}: {string.Join(", ", modelState.Value.Errors.Select(e => e.ErrorMessage))}");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var ingredients = viewModel.SelectedIngredientIds
                        .Select((id, i) => (
                            id,
                            decimal.TryParse(viewModel.IngredientAmounts.ElementAtOrDefault(i), out var amt) ? amt : 0,
                            viewModel.IngredientUnits.ElementAtOrDefault(i) ?? string.Empty))
                        .ToList();

                    await _bll.Recipes.AddFullRecipeAsync(
                        viewModel.Recipe,
                        currentUser.Id,
                        viewModel.SelectedCategoryIds!.ToArray(),
                        ingredients
                    );
                    await _bll.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating recipe");
                    ModelState.AddModelError("", "An error occurred while saving the recipe: " + ex.Message);
                }
            }

            // Repopulate dropdowns when returning the view
            await RepopulateCreateViewModelAsync(viewModel);
            return View(viewModel);
        }

        private async Task RepopulateCreateViewModelAsync(RecipeCreateViewModel viewModel)
        {
            var categories = await _bll.Categories.AllAsync();
            var ingredients = await _bll.Ingredients.AllAsync();

            viewModel.AvailableCategories = new SelectList(categories, "Id", "Name", viewModel.SelectedCategoryIds);
            viewModel.AvailableIngredients = new SelectList(ingredients, "Id", "Name", viewModel.SelectedIngredientIds);
        }


        // GET: Recipes/Edit/5
        public async Task<IActionResult> Edit(int? id)
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

            var recipe = await _bll.Recipes.GetWithDetailsAsync(id.Value, currentUser.Id);

            if (recipe == null)
            {
                return NotFound();
            }

            var categories = await _bll.Categories.AllAsync();
            var ingredients = await _bll.Ingredients.AllAsync();

            var viewModel = new RecipeEditViewModel
            {
                Recipe = _mapper.Map<RecipeUpdateDto>(recipe),
                AvailableCategories = new SelectList(categories, "Id", "Name"),
                AvailableIngredients = new SelectList(ingredients, "Id", "Name"),
                SelectedCategoryIds = recipe.Categories.Select(c => c.Id).ToList()
            };

            // Convert recipe ingredients to a dictionary for the view model
            if (recipe.Ingredients != null) // Changed from RecipeIngredients to Ingredients
            {
                foreach (var ri in recipe.Ingredients)
                {
                    viewModel.SelectedIngredients[ri.IngredientId] = new RecipeIngredientInfo
                    {
                        Amount = ri.Amount,
                        Unit = ri.Unit ?? string.Empty
                    };
                }
            }

            return View(viewModel);
        }

        // POST: Recipes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RecipeEditViewModel viewModel)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            if (id != viewModel.Recipe.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _bll.Recipes.UpdateFullRecipeAsync(
                        viewModel.Recipe,
                        currentUser.Id,
                        viewModel.SelectedCategoryIds,
                        viewModel.SelectedIngredients
                    );

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating recipe: {Message}", ex.Message);
                    ModelState.AddModelError("", "An error occurred while updating the recipe: " + ex.Message);
                }
            }

            // Repopulate dropdowns
            var categories = await _bll.Categories.AllAsync();
            var ingredientsList = await _bll.Ingredients.AllAsync();

            viewModel.AvailableCategories = new SelectList(categories, "Id", "Name", viewModel.SelectedCategoryIds);
            viewModel.AvailableIngredients =
                new SelectList(ingredientsList, "Id", "Name", viewModel.SelectedIngredients.Keys);

            return View(viewModel);
        }

        // GET: Recipes/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

            var recipe = await _bll.Recipes.GetWithDetailsAsync(id.Value, currentUser.Id);

            if (recipe == null)
            {
                return NotFound();
            }

            var recipeDto = _mapper.Map<RecipeDetailDto>(recipe);
            return View(recipeDto);
        }

        // POST: Recipes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            try
            {
                await _bll.Recipes.RemoveAsync(id, currentUser.Id);
                await _bll.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting recipe: {Message}", ex.Message);
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Recipes/GetRecipeDetails/5
        [HttpGet]
        public async Task<IActionResult> GetRecipeDetails(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            var recipe = await _bll.Recipes.GetWithDetailsAsync(id, currentUser.Id);

            if (recipe == null)
            {
                return Json(new { success = false, message = "Recipe not found" });
            }

            var recipeDto = _mapper.Map<RecipeDetailDto>(recipe);

            return Json(new
            {
                success = true,
                id = recipeDto.Id,
                name = recipeDto.Name,
                description = recipeDto.Description,
                prepTimeMinutes = recipeDto.PrepTimeMinutes,
                defaultServings = recipeDto.DefaultServings,
                caloriesPerServing = recipeDto.Nutrition?.CaloriesPerServing,
                detailsUrl = Url.Action("Details", "Recipes", new { id = recipeDto.Id })
            });
        }
    }
}