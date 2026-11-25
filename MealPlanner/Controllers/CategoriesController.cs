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
using MealPlanner.Models.ViewModels.Categories;

namespace MealPlanner.Controllers
{
    [Authorize(AuthenticationSchemes = "AuthScheme")]
    public class CategoriesController : Controller
    {
        private readonly IAppBLL _bll;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<CategoriesController> _logger;
        private readonly IMapper _mapper;

        public CategoriesController(
            IAppBLL bll, 
            UserManager<ApplicationUser> userManager,
            ILogger<CategoriesController> logger,
            IMapper mapper)
        {
            _bll = bll;
            _userManager = userManager;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Entering Categories Index action");
            
            // Get all categories using BLL service
            var categories = await _bll.Categories.AllAsync();
            
            // Get recipe counts for each category (optional, but adds value to the UI)
            var categoryRecipeCounts = new Dictionary<int, int>();
            
            foreach (var category in categories)
            {
                // This would ideally be a method in your BLL, but we can approximate for now
                var recipesInCategory = await _bll.Categories.GetRecipeCountAsync(category.Id);
                categoryRecipeCounts[category.Id] = recipesInCategory;
            }
            
            var viewModel = new CategoryIndexViewModel
            {
                Categories = categories.ToList(),
                CategoryRecipeCounts = categoryRecipeCounts
            };
    
            return View(viewModel);
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            _logger.LogInformation("Details called for category ID: {Id}", id);
    
            if (id == null)
            {
                return NotFound();
            }

            // Get category using BLL service
            var category = await _bll.Categories.FindAsync(id.Value);
                
            if (category == null)
            {
                _logger.LogWarning("Category not found for ID: {Id}", id);
                return NotFound();
            }
            
            // Get recipes in this category
            var recipesInCategory = await _bll.Categories.GetRecipesAsync(id.Value);
            var recipeCount = recipesInCategory?.Count ?? 0;
            
            var viewModel = new CategoryDetailsViewModel
            {
                Category = category,
                RecipeCount = recipeCount,
                AssociatedRecipes = recipesInCategory ?? new List<BLL.DTO.Recipe>()
            };

            return View(viewModel);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            _logger.LogInformation("Entering Categories Create GET action");
            
            var viewModel = new CategoryCreateViewModel
            {
                Category = new CategoryCreateDto()
            };
            
            return View(viewModel);
        }

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryCreateViewModel viewModel)
        {
            _logger.LogInformation("Processing Categories Create POST with data: {CategoryName}", 
                viewModel.Category.Name);
            
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
                    var bllCategory = _mapper.Map<BLL.DTO.Category>(viewModel.Category);
                    
                    // Add category using BLL service
                    var createdCategory = await _bll.Categories.AddAsync(bllCategory, currentUser.Id);
                    await _bll.SaveChangesAsync();
                    
                    _logger.LogInformation("Category created successfully with ID: {Id}", createdCategory.Id);
                    
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating category: {Message}", ex.Message);
                    ModelState.AddModelError("", "An error occurred while creating the category: " + ex.Message);
                }
            }
            else
            {
                _logger.LogWarning("Invalid model state when creating category");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogWarning("Model error: {ErrorMessage}", error.ErrorMessage);
                }
            }
    
            return View(viewModel);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            _logger.LogInformation("Entering Categories Edit GET action for id: {Id}", id);
            
            if (id == null)
            {
                _logger.LogWarning("Categories Edit called with null id");
                return NotFound();
            }

            // Get category from BLL
            var category = await _bll.Categories.FindAsync(id.Value);
            
            if (category == null)
            {
                _logger.LogWarning("Category with id {Id} not found for editing", id);
                return NotFound();
            }

            // Get recipes in this category for warning display
            var recipeCount = await _bll.Categories.GetRecipeCountAsync(id.Value);
            
            // Create view model
            var viewModel = new CategoryEditViewModel
            {
                Category = _mapper.Map<CategoryUpdateDto>(category),
                RecipeCount = recipeCount
            };
            
            return View(viewModel);
        }

        // POST: Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryEditViewModel viewModel)
        {
            _logger.LogInformation("Processing Categories Edit POST for id: {Id}", id);
            
            if (id != viewModel.Category.Id)
            {
                _logger.LogWarning("ID mismatch in Categories Edit. Path ID: {PathId}, Model ID: {ModelId}", 
                    id, viewModel.Category.Id);
                return NotFound();
            }

            // Get current user 
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            // Check if category exists
            var existingCategory = await _bll.Categories.FindAsync(id);
            if (existingCategory == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Map to BLL DTO
                    var bllCategory = _mapper.Map<BLL.DTO.Category>(viewModel.Category);
                    
                    // Update category using BLL service
                    var updatedCategory = await _bll.Categories.UpdateAsync(bllCategory, currentUser.Id);
                    await _bll.SaveChangesAsync();
                    
                    _logger.LogInformation("Category updated successfully with ID: {Id}", updatedCategory.Id);
                    
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating category {CategoryId}", viewModel.Category.Id);
                    ModelState.AddModelError("", "An error occurred while updating the category: " + ex.Message);
                }
            }
            else
            {
                _logger.LogWarning("Invalid model state when editing category {CategoryId}", viewModel.Category.Id);
            }
            
            // Repopulate recipe count on error
            viewModel.RecipeCount = await _bll.Categories.GetRecipeCountAsync(id);
            
            return View(viewModel);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            _logger.LogInformation("Entering Categories Delete GET action for id: {Id}", id);
            
            if (id == null)
            {
                return NotFound();
            }

            // Get category using BLL service
            var category = await _bll.Categories.FindAsync(id.Value);
                
            if (category == null)
            {
                _logger.LogWarning("Category with id {Id} not found for deletion", id);
                return NotFound();
            }

            // Get recipe count for warning
            var recipeCount = await _bll.Categories.GetRecipeCountAsync(id.Value);
            
            var viewModel = new CategoryDeleteViewModel
            {
                Category = category,
                RecipeCount = recipeCount
            };
            
            return View(viewModel);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _logger.LogInformation("Processing Categories DeleteConfirmed for id: {Id}", id);
            
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }
            
            try
            {
                // Delete category using BLL service
                await _bll.Categories.RemoveAsync(id, currentUser.Id);
                await _bll.SaveChangesAsync();
                
                _logger.LogInformation("Category deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category {CategoryId}", id);
                throw;
            }
            
            return RedirectToAction(nameof(Index));
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAjax(string name, string description)
        {
            _logger.LogInformation("Processing Categories CreateAjax for name: {Name}", name);
    
            if (string.IsNullOrEmpty(name))
            {
                _logger.LogWarning("CreateAjax called with empty name");
                return Json(new { success = false, message = "Category name is required" });
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Json(new { success = false, message = "User not authenticated" });
            }

            try
            {
                // Create a category DTO
                var categoryDto = new CategoryCreateDto
                {
                    Name = name,
                    Description = description ?? string.Empty
                };
                
                // Map to BLL DTO
                var bllCategory = _mapper.Map<BLL.DTO.Category>(categoryDto);
                
                // Add using BLL service
                var createdCategory = await _bll.Categories.AddAsync(bllCategory, currentUser.Id);
                await _bll.SaveChangesAsync();
                
                _logger.LogInformation("Category created successfully via AJAX with ID: {CategoryId}", createdCategory.Id);

                return Json(new { 
                    success = true, 
                    id = createdCategory.Id, 
                    name = createdCategory.Name
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category via AJAX: {Message}", ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAjax(int id, string name, string description)
        {
            _logger.LogInformation("Processing Categories EditAjax for id: {Id}", id);
            
            if (string.IsNullOrEmpty(name))
            {
                return Json(new { success = false, message = "Category name is required" });
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Json(new { success = false, message = "User not authenticated" });
            }
            
            try
            {
                // Check if category exists
                var existingCategory = await _bll.Categories.FindAsync(id);
                if (existingCategory == null)
                {
                    return Json(new { success = false, message = "Category not found" });
                }
                
                // Create update DTO
                var categoryUpdateDto = new CategoryUpdateDto
                {
                    Id = id,
                    Name = name,
                    Description = description ?? string.Empty
                };
                
                // Map to BLL DTO
                var bllCategory = _mapper.Map<BLL.DTO.Category>(categoryUpdateDto);
                
                // Update using BLL service
                var updatedCategory = await _bll.Categories.UpdateAsync(bllCategory, currentUser.Id);
                await _bll.SaveChangesAsync();
                
                return Json(new { 
                    success = true, 
                    id = updatedCategory.Id, 
                    name = updatedCategory.Name 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category via AJAX: {Message}", ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }
        
        
        
    }
}