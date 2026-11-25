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
using MealPlanner.Extensions;
using MealPlanner.Models.Inventory;
using MealPlanner.Models.ViewModels.MealPlans;
using MealPlanner.Resources.Models;

namespace MealPlanner.Controllers
{
    [Authorize(AuthenticationSchemes = "AuthScheme")]
    public class MealPlansController : Controller
    {
        private readonly IAppBLL _bll;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<MealPlansController> _logger;
        private readonly IMapper _mapper;

        public MealPlansController(
            IAppBLL bll,
            UserManager<ApplicationUser> userManager,
            ILogger<MealPlansController> logger,
            IMapper mapper)
        {
            _bll = bll;
            _userManager = userManager;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: MealPlans
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            // Get all meal plans for current user using BLL service
            var mealPlans = await _bll.MealPlans.AllAsync(currentUser.Id);

            // Return BLL DTOs directly since we're just displaying a list
            return View(mealPlans);
        }

        // GET: MealPlans/Details/5
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

            // Get meal plan with items using BLL service
            var mealPlan = await _bll.MealPlans.GetWithItemsAsync(id.Value, currentUser.Id);
            if (mealPlan == null)
            {
                return NotFound();
            }

            // Return BLL DTO directly for the details view
            return View(mealPlan);
        }

        // GET: MealPlans/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MealPlans/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BLL.DTO.MealPlan mealPlan)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var currentUser = await _userManager.GetUserAsync(User);
                    if (currentUser == null)
                    {
                        return Challenge();
                    }

                    // Add meal plan using BLL service - no need to map since we're already using BLL DTO
                    var createdMealPlan = await _bll.MealPlans.AddAsync(mealPlan, currentUser.Id);
                    await _bll.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error saving meal plan: {Message}", ex.Message);
                    ModelState.AddModelError("", "An error occurred while saving the meal plan: " + ex.Message);
                }
            }

            return View(mealPlan);
        }

        // GET: MealPlans/Edit/5
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

            // Get meal plan using BLL service
            var mealPlan = await _bll.MealPlans.FindAsync(id.Value, currentUser.Id);
            if (mealPlan == null)
            {
                return NotFound();
            }

            // Return BLL DTO directly for editing
            return View(mealPlan);
        }

        // POST: MealPlans/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BLL.DTO.MealPlan mealPlan)
        {
            if (id != mealPlan.Id)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            // Verify meal plan exists and belongs to user
            var existingMealPlan = await _bll.MealPlans.FindAsync(id, currentUser.Id);
            if (existingMealPlan == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _bll.MealPlans.UpdateAsync(mealPlan, currentUser.Id);
                    await _bll.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating meal plan: {Message}", ex.Message);
                    ModelState.AddModelError("", "An error occurred while updating the meal plan: " + ex.Message);
                    return View(mealPlan);
                }
            }

            return View(mealPlan);
        }

        // GET: MealPlans/Delete/5
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

            // Get meal plan with items using BLL service
            var mealPlan = await _bll.MealPlans.GetWithItemsAsync(id.Value, currentUser.Id);
            if (mealPlan == null)
            {
                return NotFound();
            }

            // Return BLL DTO directly for the delete confirmation view
            return View(mealPlan);
        }

        // POST: MealPlans/Delete/5
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
                // Delete meal plan using BLL service
                await _bll.MealPlans.RemoveAsync(id, currentUser.Id);
                await _bll.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting meal plan: {Message}", ex.Message);
                throw;
            }

            return RedirectToAction(nameof(Index));
        }
        
        // GET: MealPlans/AddItem/5
        public async Task<IActionResult> AddItem(int? id)
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

            // Get meal plan using BLL service
            var mealPlan = await _bll.MealPlans.FindAsync(id.Value, currentUser.Id);
            if (mealPlan == null)
            {
                return NotFound();
            }

            // Get user's recipes for dropdown using BLL service
            var userRecipes = await _bll.Recipes.AllAsync(currentUser.Id);
    
            _logger.LogInformation($"Found {userRecipes.Count} recipes for user {currentUser.Id}");
            if (userRecipes.Any())
            {
                _logger.LogInformation($"First recipe: {userRecipes.First().Name}");
            }

            // Create a ViewModel for adding an item
            var viewModel = new MealPlanItemCreateViewModel
            {
                MealPlanId = mealPlan.Id,
                MealPlanName = mealPlan.Name,
                AvailableRecipes = new SelectList(userRecipes, "Id", "Name"),
                MealPlanItem = new MealPlanItemCreateDto
                {
                    MealPlanId = mealPlan.Id,
                    PlannedDate = DateTime.Today,
                    Servings = 1
                }
            };

            return View(viewModel);
        }
        
        // POST: MealPlans/AddItem
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> AddItem(MealPlanItemCreateViewModel viewModel)
{
    _logger.LogInformation("=== AddItem POST method called ===");
    _logger.LogInformation("ModelState.IsValid: {IsValid}", ModelState.IsValid);
    
    if (viewModel?.MealPlanItem == null)
    {
        _logger.LogError("ViewModel or MealPlanItem is null");
        return BadRequest("Invalid data received");
    }
    
    _logger.LogInformation("MealPlanId: {MealPlanId}", viewModel.MealPlanItem.MealPlanId);
    _logger.LogInformation("RecipeId: {RecipeId}", viewModel.MealPlanItem.RecipeId);
    _logger.LogInformation("MealType: {MealType}", viewModel.MealPlanItem.MealType);
    _logger.LogInformation("Servings: {Servings}", viewModel.MealPlanItem.Servings);
    _logger.LogInformation("PlannedDate: {PlannedDate}", viewModel.MealPlanItem.PlannedDate);

    if (!ModelState.IsValid)
    {
        _logger.LogWarning("ModelState is invalid:");
        foreach (var error in ModelState)
        {
            _logger.LogWarning("Key: {Key}, Errors: {Errors}", 
                error.Key, 
                string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage)));
        }
        
        // Repopulate dropdown data
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            _logger.LogError("Current user is null during model validation error");
            return Challenge();
        }

        var userRecipes = await _bll.Recipes.AllAsync(currentUser.Id);
        viewModel.AvailableRecipes = new SelectList(userRecipes, "Id", "Name", viewModel.MealPlanItem.RecipeId);
        
        var mealPlan = await _bll.MealPlans.FindAsync(viewModel.MealPlanItem.MealPlanId, currentUser.Id);
        viewModel.MealPlanName = mealPlan?.Name ?? "Unknown Meal Plan";
        
        _logger.LogInformation("Returning view due to model validation errors");
        return View(viewModel);
    }

    var user = await _userManager.GetUserAsync(User);
    if (user == null)
    {
        _logger.LogError("User is null during form processing");
        return Challenge();
    }
    
    _logger.LogInformation("User authenticated: {UserId}", user.Id);

    try
    {
        // Verify the meal plan exists and belongs to user
        var mealPlan = await _bll.MealPlans.FindAsync(viewModel.MealPlanItem.MealPlanId, user.Id);
        if (mealPlan == null)
        {
            _logger.LogError("Meal plan not found or user doesn't own it. MealPlanId: {MealPlanId}, UserId: {UserId}", 
                viewModel.MealPlanItem.MealPlanId, user.Id);
            
            ModelState.AddModelError("", "Meal plan not found or you don't have permission to access it.");
            
            // Repopulate dropdown
            var userRecipes = await _bll.Recipes.AllAsync(user.Id);
            viewModel.AvailableRecipes = new SelectList(userRecipes, "Id", "Name", viewModel.MealPlanItem.RecipeId);
            viewModel.MealPlanName = "Unknown Meal Plan";
            
            return View(viewModel);
        }
        
        _logger.LogInformation("Meal plan found: {MealPlanName}", mealPlan.Name);

        // Map from ViewModel to BLL DTO
        var bllMealPlanItem = _mapper.Map<BLL.DTO.MealPlanItem>(viewModel.MealPlanItem);
        _logger.LogInformation("Mapped to BLL DTO successfully");

        // Add meal plan item using BLL service
        _logger.LogInformation("About to call AddItemAsync...");
        var addedItem = await _bll.MealPlans.AddItemAsync(viewModel.MealPlanItem.MealPlanId, bllMealPlanItem, user.Id);
        _logger.LogInformation("AddItemAsync completed, item ID: {ItemId}", addedItem.Id);
        
        _logger.LogInformation("About to save changes...");
        await _bll.SaveChangesAsync();
        _logger.LogInformation("Changes saved successfully");

        _logger.LogInformation("About to redirect to Details action with ID: {MealPlanId}", viewModel.MealPlanItem.MealPlanId);
        
        // Try different redirect approaches
        var redirectResult = RedirectToAction(nameof(Details), new { id = viewModel.MealPlanItem.MealPlanId });
        _logger.LogInformation("Redirect result created: {RedirectResult}", redirectResult.GetType().Name);
        
        return redirectResult;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error adding meal plan item: {Message}", ex.Message);
        _logger.LogError("Exception type: {ExceptionType}", ex.GetType().Name);
        _logger.LogError("Stack trace: {StackTrace}", ex.StackTrace);
        
        ModelState.AddModelError("", "An error occurred while adding the meal plan item: " + ex.Message);
        
        // Repopulate dropdown data
        var userRecipes = await _bll.Recipes.AllAsync(user.Id);
        viewModel.AvailableRecipes = new SelectList(userRecipes, "Id", "Name", viewModel.MealPlanItem.RecipeId);
        
        var mealPlan = await _bll.MealPlans.FindAsync(viewModel.MealPlanItem.MealPlanId, user.Id);
        viewModel.MealPlanName = mealPlan?.Name ?? "Unknown Meal Plan";
        
        _logger.LogInformation("Returning view due to exception");
        return View(viewModel);
    }
}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAjax(MealPlanCreateDto mealPlanDto)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    return Json(new { success = false, message = "User not authenticated" });
                }

                try
                {
                    // Map to BLL DTO
                    var bllMealPlan = _mapper.Map<BLL.DTO.MealPlan>(mealPlanDto);

                    // Add meal plan using BLL service
                    var createdMealPlan = await _bll.MealPlans.AddAsync(bllMealPlan, currentUser.Id);
                    await _bll.SaveChangesAsync();

                    return Json(new
                    {
                        success = true,
                        id = createdMealPlan.Id,
                        name = createdMealPlan.Name,
                        redirectUrl = Url.Action("Details", new { id = createdMealPlan.Id })
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating meal plan via AJAX: {Message}", ex.Message);
                    return Json(new { success = false, message = ex.Message });
                }
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return Json(new { success = false, message = string.Join(", ", errors) });
        }

        // POST: MealPlans/AddItemAjax
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddItemAjax(MealPlanItemCreateDto mealPlanItemDto)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    return Json(new { success = false, message = "User not authenticated" });
                }

                try
                {
                    // Map to BLL DTO
                    var bllMealPlanItem = _mapper.Map<BLL.DTO.MealPlanItem>(mealPlanItemDto);

                    // Add meal plan item using BLL service
                    var addedItem = await _bll.MealPlans.AddItemAsync(
                        mealPlanItemDto.MealPlanId, bllMealPlanItem, currentUser.Id);
                    await _bll.SaveChangesAsync();

                    // Get recipe name for the response
                    var recipe = await _bll.Recipes.FindAsync(mealPlanItemDto.RecipeId);
                    string recipeName = recipe?.Name ?? "Unknown Recipe";

                    return Json(new
                    {
                        success = true,
                        id = addedItem.Id,
                        recipeName = recipeName,
                        mealType = addedItem.MealType,
                        plannedDate = addedItem.PlannedDate.ToShortDateString()
                    });
                }
                catch (ArgumentException ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
                catch (UnauthorizedAccessException)
                {
                    return Json(new { success = false, message = "Not authorized" });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error adding meal plan item via AJAX: {Message}", ex.Message);
                    return Json(new { success = false, message = ex.Message });
                }
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return Json(new { success = false, message = string.Join(", ", errors) });
        }

        // POST: MealPlans/RemoveItemAjax
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveItemAjax(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Json(new { success = false, message = "User not authenticated" });
            }

            try
            {
                // Remove meal plan item using BLL service
                var result = await _bll.MealPlans.RemoveItemAsync(id, currentUser.Id);
                await _bll.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (ArgumentException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Json(new { success = false, message = "Not authorized" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing meal plan item via AJAX: {Message}", ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: MealPlans/EditItem/5
        public async Task<IActionResult> EditItem(int? id)
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

            var mealPlanItem = await _bll.MealPlans.GetMealPlanItemAsync(id.Value, currentUser.Id);
            if (mealPlanItem == null)
            {
                return NotFound();
            }

            var mealPlan = await _bll.MealPlans.GetMealPlanByItemIdAsync(id.Value, currentUser.Id);
            if (mealPlan == null)
            {
                return NotFound();
            }

            // Get user's recipes for dropdown
            var userRecipes = await _bll.Recipes.AllAsync(currentUser.Id);

            // Create a ViewModel for editing an item
            var viewModel = new MealPlanItemEditViewModel
            {
                MealPlanId = mealPlan.Id,
                MealPlanName = mealPlan.Name,
                AvailableRecipes =
                    new Microsoft.AspNetCore.Mvc.Rendering.SelectList(userRecipes, "Id", "Name", mealPlanItem.RecipeId),
                MealPlanItem = new MealPlanner.Dto.MealPlanItemUpdateDto
                {
                    Id = mealPlanItem.Id,
                    MealPlanId = mealPlanItem.MealPlanId,
                    RecipeId = mealPlanItem.RecipeId,
                    MealType = mealPlanItem.MealType,
                    PlannedDate = mealPlanItem.PlannedDate,
                    Servings = mealPlanItem.Servings
                }
            };

            return View(viewModel);
        }

        // POST: MealPlans/EditItem/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditItem(int id, MealPlanItemEditViewModel viewModel)
        {
            if (id != viewModel.MealPlanItem.Id)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            // Get the meal plan to check ownership
            var mealPlan = await _bll.MealPlans.FindAsync(viewModel.MealPlanItem.MealPlanId, currentUser.Id);
            if (mealPlan == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _bll.SaveChangesAsync();

                    return RedirectToAction(nameof(Details), new { id = viewModel.MealPlanItem.MealPlanId });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating meal plan item: {Message}", ex.Message);
                    ModelState.AddModelError("", "An error occurred while updating the meal plan item: " + ex.Message);
                }
            }

            // If we got this far, something failed, redisplay form
            var userRecipes = await _bll.Recipes.AllAsync(currentUser.Id);
            viewModel.AvailableRecipes = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                userRecipes, "Id", "Name", viewModel.MealPlanItem.RecipeId);
            viewModel.MealPlanName = mealPlan.Name;

            return View(viewModel);
        }

        // POST: MealPlans/EditItemAjax
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditItemAjax(MealPlanItemUpdateDto mealPlanItemDto)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Json(new { success = false, message = "User not authenticated" });
            }

            // Get the meal plan to check ownership
            var mealPlan = await _bll.MealPlans.FindAsync(mealPlanItemDto.MealPlanId, currentUser.Id);
            if (mealPlan == null)
            {
                return Json(new { success = false, message = "Meal plan not found" });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Map to BLL DTO
                    var bllMealPlanItem = _mapper.Map<BLL.DTO.MealPlanItem>(mealPlanItemDto);

                    // Update meal plan item using BLL service
                    var updatedItem = await _bll.MealPlans.UpdateItemAsync(bllMealPlanItem, currentUser.Id);
                    await _bll.SaveChangesAsync();

                    // Get recipe name for the response
                    var recipe = await _bll.Recipes.FindAsync(mealPlanItemDto.RecipeId);
                    string recipeName = recipe?.Name ?? "Unknown Recipe";

                    return Json(new
                    {
                        success = true,
                        id = updatedItem.Id,
                        recipeName = recipeName,
                        mealType = updatedItem.MealType,
                        plannedDate = updatedItem.PlannedDate.ToShortDateString(),
                        servings = updatedItem.Servings
                    });
                }
                catch (ArgumentException ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
                catch (UnauthorizedAccessException)
                {
                    return Json(new { success = false, message = "Not authorized" });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating meal plan item via AJAX: {Message}", ex.Message);
                    return Json(new { success = false, message = ex.Message });
                }
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return Json(new { success = false, message = string.Join(", ", errors) });
        }

        // POST: MealPlans/EditAjax
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAjax(MealPlanUpdateDto mealPlanDto)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Json(new { success = false, message = "User not authenticated" });
            }

            var existingMealPlan = await _bll.MealPlans.FindAsync(mealPlanDto.Id, currentUser.Id);
            if (existingMealPlan == null)
            {
                return Json(new { success = false, message = "Meal plan not found" });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Map to BLL DTO
                    var bllMealPlan = _mapper.Map<BLL.DTO.MealPlan>(mealPlanDto);

                    // Update meal plan using BLL service
                    var updatedMealPlan = await _bll.MealPlans.UpdateAsync(bllMealPlan, currentUser.Id);
                    await _bll.SaveChangesAsync();

                    return Json(new
                    {
                        success = true,
                        id = updatedMealPlan.Id,
                        name = updatedMealPlan.Name,
                        startDate = updatedMealPlan.StartDate,
                        endDate = updatedMealPlan.EndDate
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating meal plan via AJAX: {Message}", ex.Message);
                    return Json(new { success = false, message = "An error occurred: " + ex.Message });
                }
            }

            // If model state is invalid, collect all error messages
            var validationErrors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return Json(new { success = false, message = string.Join(", ", validationErrors) });
        }
    }
}