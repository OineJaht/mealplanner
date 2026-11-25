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
using MealPlanner.Dto;
using MealPlanner.Models.Inventory;
using MealPlanner.Models.ViewModels.Ingredients;

namespace MealPlanner.Controllers
{
    [Authorize(AuthenticationSchemes = "AuthScheme")]
    public class IngredientsController : Controller
    {
        private readonly IAppBLL _bll;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<IngredientsController> _logger;
        private readonly IMapper _mapper;

        public IngredientsController(
            IAppBLL bll, 
            UserManager<ApplicationUser> userManager,
            ILogger<IngredientsController> logger,
            IMapper mapper)
        {
            _bll = bll;
            _userManager = userManager;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: Ingredients
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Entering Ingredients Index action");
    
            // Get all ingredients using BLL service
            var ingredients = await _bll.Ingredients.AllAsync();
            // var ingredientDtos = _mapper.Map<List<IngredientDto>>(ingredients);
            
            return View(ingredients);
        }

        // GET: Ingredients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            _logger.LogInformation("Details called for ingredient ID: {Id}", id);
    
            if (id == null)
            {
                return NotFound();
            }
            
            // Get ingredient with nutrition using BLL service
            var ingredient = await _bll.Ingredients.GetWithNutritionAsync(id.Value);
        
            if (ingredient == null)
            {
                _logger.LogWarning("Ingredient not found for ID: {Id}", id);
                return NotFound();
            }
            
            // Log some information about the ingredient
            _logger.LogInformation($"Ingredient ID: {ingredient.Id}, Name: {ingredient.Name}");
            _logger.LogInformation($"Has Nutrition: {ingredient.Nutrition != null}");
            if (ingredient.Nutrition != null)
            {
                _logger.LogInformation($"Nutrition data: Calories={ingredient.Nutrition.CaloriesPer100g}, Protein={ingredient.Nutrition.ProteinG}");
            }

            return View(ingredient);
        }

        // GET: Ingredients/Create
        public IActionResult Create()
        {
            _logger.LogInformation("Entering Ingredients Create GET action");
            return View();
        }

        // POST: Ingredients/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IngredientCreateDto ingredientDto)
        {
            _logger.LogInformation("Processing Ingredients Create POST with data: {IngredientName}", ingredientDto.Name);
            
            if (ModelState.IsValid)
            {
                try 
                {
                    // Get current user to use as creator
                    var currentUser = await _userManager.GetUserAsync(User);
                    if (currentUser == null)
                    {
                        return Challenge();
                    }
                    
                    // Map to BLL DTO
                    var bllIngredient = _mapper.Map<BLL.DTO.Ingredient>(ingredientDto);
                    
                    // If Nutrition data is included, handle it appropriately
                    // This would be based on your BLL design
                    BLL.DTO.IngredientNutrition? bllNutrition = null;
                    if (ingredientDto.Nutrition != null)
                    {
                        bllNutrition = _mapper.Map<BLL.DTO.IngredientNutrition>(ingredientDto.Nutrition);
                    }
                    
                    // Add the ingredient using BLL service
                    var createdIngredient = await _bll.Ingredients.AddAsync(bllIngredient, currentUser.Id);
                    
                    // If nutrition data was provided, you might need a dedicated method for this
                    // Depending on your BLL design
                    if (bllNutrition != null)
                    {
                        // This might be something like:
                        // await _bll.Ingredients.AddNutritionAsync(createdIngredient.Id, bllNutrition);
                    }
                    
                    await _bll.SaveChangesAsync();
                    
                    _logger.LogInformation("Ingredient saved successfully with ID: {IngredientId}", createdIngredient.Id);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error saving ingredient");
                    ModelState.AddModelError("", "An error occurred while saving the ingredient: " + ex.Message);
                }
            }
            else
            {
                _logger.LogWarning("Invalid model state when creating ingredient");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogWarning("Model error: {ErrorMessage}", error.ErrorMessage);
                }
            }
            var BllIngredient = _mapper.Map<BLL.DTO.Ingredient>(ingredientDto);
            return View(BllIngredient);
        }

        // GET: Ingredients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            _logger.LogInformation("Entering Ingredients Edit GET action for id: {Id}", id);
            
            if (id == null)
            {
                _logger.LogWarning("Ingredients Edit called with null id");
                return NotFound();
            }

            // Get ingredient with nutrition using BLL service
            var ingredient = await _bll.Ingredients.GetWithNutritionAsync(id.Value);
            
            _logger.LogInformation($"Ingredient ID: {ingredient?.Id}, Has Nutrition: {ingredient?.Nutrition != null}");
            
            if (ingredient == null)
            {
                _logger.LogWarning("Ingredient with id {Id} not found for editing", id);
                return NotFound();
            }

            // Map to DTO for the view
            var ingredientUpdateDto = _mapper.Map<IngredientUpdateDto>(ingredient);
            
            // Create nutrition DTO for the view or use a default if not available
            var nutritionUpdateDto = ingredient.Nutrition != null ? 
                _mapper.Map<IngredientNutritionUpdateDto>(ingredient.Nutrition) : 
                new IngredientNutritionUpdateDto { Id = 0 };
                
            // Create the view model rather than using ViewBag
            var viewModel = new IngredientEditViewModel
            {
                Ingredient = _mapper.Map<IngredientUpdateDto>(ingredient),
                Nutrition = ingredient.Nutrition != null ? 
                    _mapper.Map<IngredientNutritionUpdateDto>(ingredient.Nutrition) : 
                    new IngredientNutritionUpdateDto { IngredientId = id.Value }
            };
            
            return View(viewModel);
        }
        
        public async Task<IActionResult> TestNutrition(int id)
        {
            // Use BLL service to get ingredient with nutrition
            var ingredient = await _bll.Ingredients.GetWithNutritionAsync(id);
            
            return Json(new { 
                IngredientExists = ingredient != null,
                NutritionExists = ingredient?.Nutrition != null,
                NutritionData = ingredient?.Nutrition,
                IngredientName = ingredient?.Name
            });
        }

        // POST: Ingredients/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IngredientEditViewModel viewModel)
        {
            _logger.LogInformation("Processing Ingredients Edit POST for id: {Id}", id);
            
            if (id != viewModel.Ingredient.Id)
            {
                _logger.LogWarning("ID mismatch in Ingredients Edit. Path ID: {PathId}, Model ID: {ModelId}", id, viewModel.Ingredient.Id);
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            // Check if ingredient exists
            var existingIngredient = await _bll.Ingredients.FindAsync(id);
            if (existingIngredient == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Map to BLL DTO
                    var bllIngredient = _mapper.Map<BLL.DTO.Ingredient>(viewModel.Ingredient);
                    
                    // Update the ingredient
                    var updatedIngredient = await _bll.Ingredients.UpdateAsync(bllIngredient, currentUser.Id);
                    
                    // Handle nutrition data update
                    // This would depend on your BLL design
                    if (viewModel.Nutrition != null)
                    {
                        var bllNutrition = _mapper.Map<BLL.DTO.IngredientNutrition>(viewModel.Nutrition);
                        
                        // You might have a method like this:
                        // await _bll.Ingredients.UpdateNutritionAsync(id, bllNutrition);
                    }
                    
                    await _bll.SaveChangesAsync();
                    _logger.LogInformation("Ingredient updated successfully");
                    
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating ingredient {IngredientId}", viewModel.Ingredient.Id);
                    ModelState.AddModelError("", "An error occurred while updating the ingredient");
                    
                    return View(viewModel);
                }
            }
            
            _logger.LogWarning("Invalid model state when editing ingredient {IngredientId}", viewModel.Ingredient.Id);
            return View(viewModel);
        }
    


        // GET: Ingredients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            _logger.LogInformation("Entering Ingredients Delete GET action for id: {Id}", id);
            
            if (id == null)
            {
                return NotFound();
            }

            // Get ingredient with details using BLL service
            var ingredient = await _bll.Ingredients.GetWithNutritionAsync(id.Value);
                
            if (ingredient == null)
            {
                _logger.LogWarning("Ingredient with id {Id} not found for deletion", id);
                return NotFound();
            }

            var ingredientDetailDto = _mapper.Map<IngredientDetailDto>(ingredient);
            var bllIngredientDetail = _mapper.Map<MealPlanner.BLL.DTO.IngredientDetail>(ingredient);
            return View(bllIngredientDetail);
        }

        // POST: Ingredients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _logger.LogInformation("Processing Ingredients DeleteConfirmed for id: {Id}", id);
            
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }
            
            try
            {
                // Delete ingredient using BLL service
                await _bll.Ingredients.RemoveAsync(id, currentUser.Id);
                await _bll.SaveChangesAsync();
                
                _logger.LogInformation("Ingredient deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting ingredient {IngredientId}", id);
                throw;
            }
            
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAjax(string name, string category)
        {
            _logger.LogInformation("Processing Ingredients CreateAjax for name: {Name}", name);
    
            if (string.IsNullOrEmpty(name))
            {
                _logger.LogWarning("CreateAjax called with empty name");
                return Json(new { success = false, message = "Ingredient name is required" });
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Json(new { success = false, message = "User not authenticated" });
            }

            try
            {
                // Create an ingredient DTO
                var ingredientDto = new IngredientCreateDto
                {
                    Name = name,
                    Category = category ?? string.Empty
                };
                
                // Map to BLL DTO
                var bllIngredient = _mapper.Map<BLL.DTO.Ingredient>(ingredientDto);
                
                // Add using BLL service
                var createdIngredient = await _bll.Ingredients.AddAsync(bllIngredient, currentUser.Id);
                await _bll.SaveChangesAsync();
                
                _logger.LogInformation("Ingredient created successfully via AJAX with ID: {IngredientId}", createdIngredient.Id);

                return Json(new { 
                    success = true, 
                    id = createdIngredient.Id, 
                    name = createdIngredient.Name
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating ingredient via AJAX");
                return Json(new { success = false, message = ex.Message });
            }
        }
        
        
        
    }
}