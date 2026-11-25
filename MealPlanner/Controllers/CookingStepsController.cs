using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using AutoMapper;
using MealPlanner.BLL.Services;
using MealPlanner.Dto;
using MealPlanner.Models.Inventory;
using MealPlanner.Models.ViewModels.CookingSteps;

namespace MealPlanner.Controllers
{
    [Authorize(AuthenticationSchemes = "AuthScheme")]
    public class CookingStepsController : Controller
    {
        private readonly IAppBLL _bll;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<CookingStepsController> _logger;
        private readonly IMapper _mapper;

        public CookingStepsController(
            IAppBLL bll,
            UserManager<ApplicationUser> userManager,
            ILogger<CookingStepsController> logger,
            IMapper mapper)
        {
            _bll = bll;
            _userManager = userManager;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: CookingSteps/Index/5 (RecipeId)
        public async Task<IActionResult> Index(int? id)
        {
            _logger.LogInformation("Entering CookingSteps Index action for id: {Id}", id);

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

            var cookingStepDtos = _mapper.Map<List<CookingStepDto>>(recipe.CookingSteps);

            var viewModel = new CookingStepIndexViewModel
            {
                CookingSteps = cookingStepDtos,
                RecipeId = id.Value,
                RecipeName = recipe.Name
            };

            return View(viewModel);
        }

        // GET: CookingSteps/Create/5 (RecipeId)
        public async Task<IActionResult> Create(int? id)
        {
            _logger.LogInformation("Entering CookingSteps Create GET action for id: {Id}", id);

            if (id == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            var recipe = await _bll.Recipes.FindAsync(id.Value, currentUser.Id);
            if (recipe == null)
            {
                return NotFound();
            }

            // Get recipe with details to count existing steps
            var recipeWithDetails = await _bll.Recipes.GetWithDetailsAsync(id.Value, currentUser.Id);
            int nextStepNumber = 1;
            if (recipeWithDetails?.CookingSteps != null && recipeWithDetails.CookingSteps.Any())
            {
                nextStepNumber = recipeWithDetails.CookingSteps.Max(cs => cs.StepNumber) + 1;
            }

            var cookingStepDto = new CookingStepCreateDto
            {
                RecipeId = recipe.Id,
                StepNumber = nextStepNumber
            };

            var viewModel = new CookingStepCreateViewModel
            {
                CookingStep = cookingStepDto,
                RecipeId = recipe.Id,
                RecipeName = recipe.Name
            };

            return View(viewModel);
        }

        // POST: CookingSteps/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CookingStepCreateViewModel viewModel, bool continueAdding = false)
        {
            _logger.LogInformation("Create POST called with RecipeId: {RecipeId}", viewModel.CookingStep.RecipeId);

            if (viewModel.CookingStep == null)
            {
                return BadRequest();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                _logger.LogWarning("User not authenticated when creating cooking step");
                return Challenge();
            }

            var recipe = await _bll.Recipes.FindAsync(viewModel.CookingStep.RecipeId, currentUser.Id);
            if (recipe == null)
            {
                _logger.LogWarning("Recipe not found: {RecipeId}", viewModel.CookingStep.RecipeId);
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid ModelState when creating cooking step");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogWarning("Model error: {Error}", error.ErrorMessage);
                }

                viewModel.RecipeId = viewModel.CookingStep.RecipeId;
                viewModel.RecipeName = recipe.Name;
                return View(viewModel);
            }

            try
            {
                // Map to BLL DTO
                var bllCookingStep = _mapper.Map<BLL.DTO.CookingStep>(viewModel.CookingStep);

                // Add cooking step using BLL service
                _logger.LogInformation("Adding cooking step via BLL service");
                var createdStep = await _bll.Recipes.AddCookingStepAsync(bllCookingStep, currentUser.Id);
                await _bll.SaveChangesAsync();

                _logger.LogInformation("Cooking step saved successfully with ID: {Id}", createdStep.Id);

                // Redirect based on whether "Add & Create Another" was clicked
                if (continueAdding)
                {
                    return RedirectToAction(nameof(Create), new { id = createdStep.RecipeId });
                }

                return RedirectToAction(nameof(Index), new { id = createdStep.RecipeId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating cooking step for recipe ID: {RecipeId}",
                    viewModel.CookingStep.RecipeId);
                ModelState.AddModelError("", "An error occurred while saving the cooking step: " + ex.Message);

                viewModel.RecipeId = viewModel.CookingStep.RecipeId;
                viewModel.RecipeName = recipe.Name;
                return View(viewModel);
            }
        }

        // GET: CookingSteps/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            _logger.LogInformation("Entering CookingSteps Edit GET action for id: {Id}", id);

            if (id == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            var cookingStep = await _bll.Recipes.GetCookingStepAsync(id.Value, currentUser.Id);
            if (cookingStep == null)
            {
                return NotFound();
            }

            var recipe = await _bll.Recipes.FindAsync(cookingStep.RecipeId, currentUser.Id);
            if (recipe == null)
            {
                return NotFound();
            }

            var cookingStepDto = _mapper.Map<CookingStepUpdateDto>(cookingStep);

            var viewModel = new CookingStepEditViewModel
            {
                CookingStep = cookingStepDto,
                RecipeId = cookingStep.RecipeId,
                RecipeName = recipe.Name ?? "Recipe"
            };

            return View(viewModel);
        }

        // POST: CookingSteps/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CookingStepEditViewModel viewModel)
        {
            _logger.LogInformation("Edit POST called for id: {Id}", id);

            if (id != viewModel.CookingStep.Id)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            var recipe = await _bll.Recipes.FindAsync(viewModel.CookingStep.RecipeId, currentUser.Id);
            if (recipe == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var bllCookingStep = _mapper.Map<BLL.DTO.CookingStep>(viewModel.CookingStep);
                    await _bll.Recipes.UpdateCookingStepAsync(bllCookingStep, currentUser.Id);
                    await _bll.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating cooking step {StepId}", viewModel.CookingStep.Id);
                    var exists = await _bll.Recipes.GetCookingStepAsync(viewModel.CookingStep.Id, currentUser.Id);
                    if (exists == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { id = viewModel.CookingStep.RecipeId });
            }

            viewModel.RecipeId = viewModel.CookingStep.RecipeId;
            viewModel.RecipeName = recipe.Name;

            return View(viewModel);
        }

        // GET: CookingSteps/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            _logger.LogInformation("Entering CookingSteps Delete GET action for id: {Id}", id);

            if (id == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            var cookingStep = await _bll.Recipes.GetCookingStepAsync(id.Value, currentUser.Id);
            if (cookingStep == null)
            {
                return NotFound();
            }

            var recipe = await _bll.Recipes.FindAsync(cookingStep.RecipeId, currentUser.Id);
            if (recipe == null)
            {
                return NotFound();
            }

            var cookingStepDto = _mapper.Map<CookingStepDto>(cookingStep);

            var viewModel = new CookingStepDeleteViewModel
            {
                CookingStep = cookingStepDto,
                RecipeId = cookingStep.RecipeId,
                RecipeName = recipe.Name ?? "Recipe"
            };

            return View(viewModel);
        }

        // POST: CookingSteps/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _logger.LogInformation("Processing CookingSteps DeleteConfirmed for id: {Id}", id);

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            // Get cooking step using BLL service
            var cookingStep = await _bll.Recipes.GetCookingStepAsync(id, currentUser.Id);
            if (cookingStep == null)
            {
                return NotFound();
            }

            // Get recipe for verification
            var recipe = await _bll.Recipes.FindAsync(cookingStep.RecipeId, currentUser.Id);
            if (recipe == null)
            {
                return NotFound();
            }

            int recipeId = cookingStep.RecipeId;

            // Remove cooking step using BLL service
            // Assuming you have a method like RemoveCookingStepAsync in your recipe service
            await _bll.Recipes.RemoveCookingStepAsync(id, currentUser.Id);
            await _bll.SaveChangesAsync();

            // Reorder remaining steps if needed
            // This would ideally be handled by the BLL layer
            await _bll.Recipes.ReorderCookingStepsAsync(recipeId, currentUser.Id);

            return RedirectToAction(nameof(Index), new { id = recipeId });
        }

        // POST: CookingSteps/ReorderSteps
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReorderSteps(int recipeId, int[] stepIds)
        {
            _logger.LogInformation("ReorderSteps called for recipe ID: {RecipeId}", recipeId);

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            // Get recipe to verify ownership
            var recipe = await _bll.Recipes.FindAsync(recipeId, currentUser.Id);
            if (recipe == null)
            {
                return NotFound();
            }

            if (stepIds != null && stepIds.Length > 0)
            {
                // Reorder cooking steps using BLL service
                // Assuming you have a method like ReorderCookingStepsAsync in your recipe service
                await _bll.Recipes.ReorderCookingStepsAsync(recipeId, stepIds, currentUser.Id);
                await _bll.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index), new { id = recipeId });
        }
    }
}