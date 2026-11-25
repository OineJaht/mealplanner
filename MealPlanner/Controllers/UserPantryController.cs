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
using MealPlanner.Models.ViewModels.UserPantry;

namespace MealPlanner.Controllers
{
    [Authorize(AuthenticationSchemes = "AuthScheme")]
    public class UserPantryController : Controller
    {
        private readonly IAppBLL _bll;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UserPantryController> _logger;
        private readonly IMapper _mapper;

        public UserPantryController(
            IAppBLL bll,
            UserManager<ApplicationUser> userManager,
            ILogger<UserPantryController> logger,
            IMapper mapper)
        {
            _bll = bll;
            _userManager = userManager;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: UserPantry
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Challenge();

            var pantryItems = await _bll.UserPantry.GetUserItemsAsync(currentUser.Id);
            var pantryItemDtos = _mapper.Map<List<MealPlanner.Dto.UserPantryItemDto>>(pantryItems);
            
            var lowStockItems = await _bll.UserPantry.GetLowStockItemsAsync(currentUser.Id, 5);
    
            var viewModel = new UserPantryIndexViewModel
            {
                Items = pantryItemDtos,
                LowStockCount = lowStockItems.Count 
            };

            return View(viewModel);
        }

        // GET: UserPantry/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Challenge();

            var pantryItem = await _bll.UserPantry.FindAsync(id, currentUser.Id);
            if (pantryItem == null) return NotFound();

            var ingredient = await _bll.Ingredients.GetWithNutritionAsync(pantryItem.IngredientId);

            var shoppingLists = await _bll.ShoppingLists.GetAllAsync(currentUser.Id);

            // Map the pantry item first
            var pantryItemDto = _mapper.Map<UserPantryItemDto>(pantryItem);
    
            // Then manually set the ingredient name and category from the loaded ingredient
            pantryItemDto.IngredientName = ingredient?.Name ?? "Unknown";
            pantryItemDto.IngredientCategory = ingredient?.Category ?? string.Empty;

            var viewModel = new UserPantryDetailsViewModel
            {
                PantryItem = pantryItemDto, // Use the updated DTO
                IngredientCategory = ingredient?.Category ?? string.Empty,
                Nutrition = _mapper.Map<IngredientNutritionDto>(ingredient?.Nutrition),
                AvailableShoppingLists = shoppingLists.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name
                })
            };

            return View(viewModel);
        }

        // GET: UserPantry/Create
        public async Task<IActionResult> Create()
        {
            _logger.LogInformation("Entering UserPantry Create GET action");
    
            try
            {
                var ingredients = await _bll.Ingredients.GetAllAsync();
                _logger.LogInformation("Found {Count} ingredients for dropdown", ingredients.Count);
        
                var viewModel = new UserPantryCreateViewModel
                {
                    PantryItem = new UserPantryItemCreateDto
                    {
                        ExpiryDate = DateTime.Today.AddDays(30), // Default 30 days from now
                        Unit = "pcs", // Default unit
                        AmountInStock = 1 // Default amount
                    },
                    AvailableIngredients = new SelectList(ingredients, "Id", "Name")
                };
        
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading Create pantry item view");
                throw;
            }
        }

        // POST: UserPantry/Create
// Updated Create method with ingredient validation
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(UserPantryCreateViewModel viewModel)
{
    _logger.LogInformation("Processing UserPantry Create POST");
    
    // Remove validation error for dropdown
    ModelState.Remove("AvailableIngredients");
    
    _logger.LogInformation("Received data: IngredientId={IngredientId}, Amount={Amount}, Unit={Unit}, Location={Location}, ExpiryDate={ExpiryDate}", 
        viewModel.PantryItem?.IngredientId, 
        viewModel.PantryItem?.AmountInStock, 
        viewModel.PantryItem?.Unit, 
        viewModel.PantryItem?.Location, 
        viewModel.PantryItem?.ExpiryDate);

    if (!ModelState.IsValid)
    {
        _logger.LogWarning("ModelState is invalid");
        foreach (var modelError in ModelState)
        {
            foreach (var error in modelError.Value.Errors)
            {
                _logger.LogWarning("Model validation error in {Key}: {Error}", modelError.Key, error.ErrorMessage);
            }
        }
        
        var ingredients = await _bll.Ingredients.GetAllAsync();
        viewModel.AvailableIngredients = new SelectList(ingredients, "Id", "Name", viewModel.PantryItem?.IngredientId);
        return View(viewModel);
    }

    var currentUser = await _userManager.GetUserAsync(User);
    if (currentUser == null)
    {
        _logger.LogWarning("Current user not found");
        return Challenge();
    }

    try
    {
        // CRITICAL: Validate that the ingredient ID exists
        _logger.LogInformation("Validating ingredient ID: {IngredientId}", viewModel.PantryItem!.IngredientId);
        
        var selectedIngredient = await _bll.Ingredients.FindAsync(viewModel.PantryItem.IngredientId);
        if (selectedIngredient == null)
        {
            _logger.LogError("Ingredient with ID {IngredientId} not found", viewModel.PantryItem.IngredientId);
            ModelState.AddModelError("PantryItem.IngredientId", "Selected ingredient not found");
            
            var ingredients = await _bll.Ingredients.GetAllAsync();
            viewModel.AvailableIngredients = new SelectList(ingredients, "Id", "Name", viewModel.PantryItem?.IngredientId);
            return View(viewModel);
        }

        _logger.LogInformation("Found ingredient: {IngredientName} (Category: {Category})", 
            selectedIngredient.Name, selectedIngredient.Category);

        // Create the pantry item DTO with ONLY the necessary data
        var bllPantryItem = new BLL.DTO.UserPantryItem
        {
            // Only set the foreign key ID - DO NOT set navigation properties
            IngredientId = viewModel.PantryItem.IngredientId,
            AmountInStock = viewModel.PantryItem.AmountInStock,
            Unit = viewModel.PantryItem.Unit ?? "pcs",
            Location = viewModel.PantryItem.Location ?? string.Empty,
            ExpiryDate = viewModel.PantryItem.ExpiryDate,
            UserId = currentUser.Id,
            
            // Set these for display purposes but they won't be saved to DB
            IngredientName = selectedIngredient.Name,
            IngredientCategory = selectedIngredient.Category
        };
        
        _logger.LogInformation("Calling BLL AddAsync with UserId: {UserId}", currentUser.Id);
        
        var createdItem = await _bll.UserPantry.AddAsync(bllPantryItem, currentUser.Id);
        
        _logger.LogInformation("BLL AddAsync completed, saving changes");
        
        await _bll.SaveChangesAsync();
        
        _logger.LogInformation("Successfully created pantry item with ID: {ItemId}", createdItem.Id);
        
        TempData["SuccessMessage"] = "Pantry item added successfully!";
        
        return RedirectToAction(nameof(Index));
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error creating pantry item: {Message}", ex.Message);
        
        ModelState.AddModelError("", $"An error occurred while adding the pantry item: {ex.Message}");
        
        var ingredients = await _bll.Ingredients.GetAllAsync();
        viewModel.AvailableIngredients = new SelectList(ingredients, "Id", "Name", viewModel.PantryItem?.IngredientId);
        
        return View(viewModel);
    }
}

        // GET: UserPantry/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Challenge();

            var pantryItem = await _bll.UserPantry.FindAsync(id.Value, currentUser.Id);
            if (pantryItem == null) return NotFound();

            var dto = _mapper.Map<UserPantryItemUpdateDto>(pantryItem);
            var ingredients = await _bll.Ingredients.GetAllAsync();

            var viewModel = new UserPantryEditViewModel
            {
                PantryItem = dto,
                AvailableIngredients = new SelectList(ingredients, "Id", "Name", pantryItem.IngredientId)
            };

            return View(viewModel);
        }

        // POST: UserPantry/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserPantryEditViewModel viewModel)
        {
            if (id != viewModel.PantryItem.Id) return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Challenge();

            if (!ModelState.IsValid)
            {
                var ingredients = await _bll.Ingredients.GetAllAsync();
                viewModel.AvailableIngredients =
                    new SelectList(ingredients, "Id", "Name", viewModel.PantryItem.IngredientId);
                return View(viewModel);
            }

            var existingItem = await _bll.UserPantry.FindAsync(id, currentUser.Id);
            if (existingItem == null) return NotFound();

            _mapper.Map(viewModel.PantryItem, existingItem);
            await _bll.UserPantry.UpdateAsync(existingItem, currentUser.Id);
            await _bll.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: UserPantry/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Challenge();

            var pantryItem = await _bll.UserPantry.FindAsync(id.Value, currentUser.Id);
            if (pantryItem == null) return NotFound();

            var ingredient = await _bll.Ingredients.GetWithNutritionAsync(pantryItem.IngredientId);

            var dto = _mapper.Map<UserPantryItemDto>(pantryItem);
            dto.IngredientName = ingredient?.Name;
            dto.IngredientCategory = ingredient?.Category;

            return View(dto);
        }

        // POST: UserPantry/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Challenge();

            await _bll.UserPantry.RemoveAsync(id, currentUser.Id);
            await _bll.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: UserPantry/LowStock
        public async Task<IActionResult> LowStock()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            // Get low stock items using BLL service
            var lowStockItems = await _bll.UserPantry.GetLowStockItemsAsync(currentUser.Id, 5);
            var lowStockItemDtos = _mapper.Map<List<UserPantryItemDto>>(lowStockItems);

            // Get shopping lists for dropdown
            var shoppingLists = await _bll.ShoppingLists.GetAllAsync(currentUser.Id);
            var shoppingListSelectItems = shoppingLists.Select(sl => new SelectListItem
            {
                Value = sl.Id.ToString(),
                Text = sl.Name
            }).ToList();

            var viewModel = new UserPantryLowStockViewModel
            {
                Items = lowStockItemDtos,
                AvailableShoppingLists = shoppingListSelectItems
            };

            return View(viewModel);
        }

        // GET: UserPantry/Expiring
        public async Task<IActionResult> Expiring()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            // Get expiring items using BLL service
            var expiringItems = await _bll.UserPantry.GetExpiringItemsAsync(currentUser.Id, 7);
            var itemDtos = _mapper.Map<List<UserPantryItemDto>>(expiringItems);

            // Get shopping lists for the dropdown
            var shoppingLists = await _bll.ShoppingLists.AllAsync(currentUser.Id);
            var shoppingListSelectItems = shoppingLists.Select(sl => new SelectListItem
            {
                Value = sl.Id.ToString(),
                Text = sl.Name
            }).ToList();

            var viewModel = new ExpiringPantryItemsViewModel
            {
                Items = itemDtos,
                AvailableShoppingLists = shoppingListSelectItems
            };

            return View(viewModel);
        }

        // POST: UserPantry/ExtendExpiry
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExtendExpiry(int id, DateTime newExpiryDate)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            try
            {
                var pantryItem = await _bll.UserPantry.FindAsync(id, currentUser.Id);
                if (pantryItem == null)
                {
                    return NotFound();
                }

                // Update the expiry date
                pantryItem.ExpiryDate = newExpiryDate;
                await _bll.UserPantry.UpdateAsync(pantryItem, currentUser.Id);
                await _bll.SaveChangesAsync();

                TempData["SuccessMessage"] =
                    $"Extended expiry date for {pantryItem.IngredientName} to {newExpiryDate.ToShortDateString()}";
                return RedirectToAction(nameof(Expiring));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extending expiry date for pantry item {ItemId}", id);
                TempData["ErrorMessage"] = "An error occurred while extending the expiry date.";
                return RedirectToAction(nameof(Expiring));
            }
        }

        // POST: UserPantry/RemoveExpired
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveExpired()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            try
            {
                // Get all expired items
                var allItems = await _bll.UserPantry.GetUserItemsAsync(currentUser.Id);
                var expiredItems = allItems.Where(item => item.ExpiryDate < DateTime.Today).ToList();

                // Remove expired items
                int removedCount = 0;
                foreach (var item in expiredItems)
                {
                    await _bll.UserPantry.RemoveAsync(item.Id, currentUser.Id);
                    removedCount++;
                }

                await _bll.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Removed {removedCount} expired items from your pantry.";
                return RedirectToAction(nameof(Expiring));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing expired items");
                TempData["ErrorMessage"] = "An error occurred while removing expired items.";
                return RedirectToAction(nameof(Expiring));
            }
        }

        // POST: UserPantry/AddMultipleToShoppingList
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMultipleToShoppingList(int[] selectedItems, int shoppingListId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            if (selectedItems == null || selectedItems.Length == 0)
            {
                TempData["ErrorMessage"] = "No items selected.";
                return RedirectToAction(nameof(Expiring));
            }

            try
            {
                // Verify shopping list exists and belongs to user
                var shoppingList = await _bll.ShoppingLists.FindAsync(shoppingListId, currentUser.Id);
                if (shoppingList == null)
                {
                    TempData["ErrorMessage"] = "Shopping list not found.";
                    return RedirectToAction(nameof(Expiring));
                }

                int addedCount = 0;
                foreach (var itemId in selectedItems)
                {
                    var pantryItem = await _bll.UserPantry.FindAsync(itemId, currentUser.Id);
                    if (pantryItem != null)
                    {
                        // Create shopping list item
                        var shoppingListItem = new BLL.DTO.ShoppingListItem
                        {
                            ShoppingListId = shoppingListId,
                            IngredientId = pantryItem.IngredientId,
                            Amount = 1, // Default amount
                            Purchased = false
                        };

                        await _bll.ShoppingLists.AddItemAsync(shoppingListId, shoppingListItem, currentUser.Id);
                        addedCount++;
                    }
                }

                await _bll.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Added {addedCount} items to your shopping list.";
                return RedirectToAction(nameof(Expiring));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding multiple items to shopping list");
                TempData["ErrorMessage"] = "An error occurred while adding items to the shopping list.";
                return RedirectToAction(nameof(Expiring));
            }
        }

// POST: UserPantry/ExtendMultipleExpiry
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExtendMultipleExpiry(int[] selectedItems, DateTime newExpiryDate)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            if (selectedItems == null || selectedItems.Length == 0)
            {
                TempData["ErrorMessage"] = "No items selected.";
                return RedirectToAction(nameof(Expiring));
            }

            try
            {
                int updatedCount = 0;
                foreach (var itemId in selectedItems)
                {
                    var pantryItem = await _bll.UserPantry.FindAsync(itemId, currentUser.Id);
                    if (pantryItem != null)
                    {
                        pantryItem.ExpiryDate = newExpiryDate;
                        await _bll.UserPantry.UpdateAsync(pantryItem, currentUser.Id);
                        updatedCount++;
                    }
                }

                await _bll.SaveChangesAsync();

                TempData["SuccessMessage"] =
                    $"Extended expiry date for {updatedCount} items to {newExpiryDate.ToShortDateString()}";
                return RedirectToAction(nameof(Expiring));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extending expiry dates for multiple items");
                TempData["ErrorMessage"] = "An error occurred while extending expiry dates.";
                return RedirectToAction(nameof(Expiring));
            }
        }
        
        // POST: UserPantry/MarkMultipleAsUsed
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkMultipleAsUsed(int[] selectedItems)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            if (selectedItems == null || selectedItems.Length == 0)
            {
                TempData["ErrorMessage"] = "No items selected.";
                return RedirectToAction(nameof(Expiring));
            }

            try
            {
                int removedCount = 0;
                foreach (var itemId in selectedItems)
                {
                    var pantryItem = await _bll.UserPantry.FindAsync(itemId, currentUser.Id);
                    if (pantryItem != null)
                    {
                        // Mark as used by setting amount to 0 or removing entirely
                        await _bll.UserPantry.RemoveAsync(itemId, currentUser.Id);
                        removedCount++;
                    }
                }

                await _bll.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Marked {removedCount} items as used up and removed from pantry.";
                return RedirectToAction(nameof(Expiring));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking multiple items as used");
                TempData["ErrorMessage"] = "An error occurred while marking items as used.";
                return RedirectToAction(nameof(Expiring));
            }
        }
        
        // POST: UserPantry/DeleteMultiple
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMultiple(int[] selectedItems)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            if (selectedItems == null || selectedItems.Length == 0)
            {
                TempData["ErrorMessage"] = "No items selected.";
                return RedirectToAction(nameof(Expiring));
            }

            try
            {
                int deletedCount = 0;
                foreach (var itemId in selectedItems)
                {
                    var result = await _bll.UserPantry.RemoveAsync(itemId, currentUser.Id);
                    if (result)
                    {
                        deletedCount++;
                    }
                }

                await _bll.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Deleted {deletedCount} items from your pantry.";
                return RedirectToAction(nameof(Expiring));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting multiple items");
                TempData["ErrorMessage"] = "An error occurred while deleting items.";
                return RedirectToAction(nameof(Expiring));
            }
        }

        // POST: UserPantry/AddToShoppingList
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToShoppingList(int id, int shoppingListId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            // Find pantry item using BLL service
            var pantryItem = await _bll.UserPantry.FindAsync(id, currentUser.Id);
            if (pantryItem == null)
            {
                return NotFound();
            }

            // Get shopping list to verify ownership
            var shoppingList = await _bll.ShoppingLists.FindAsync(shoppingListId, currentUser.Id);
            if (shoppingList == null)
            {
                return NotFound();
            }

            try
            {
                // Create shopping list item
                var itemDto = new BLL.DTO.ShoppingListItem
                {
                    ShoppingListId = shoppingListId,
                    IngredientId = pantryItem.IngredientId,
                    Amount = 1, // Default amount
                    Purchased = false
                };

                // Add to shopping list using BLL service
                await _bll.ShoppingLists.AddItemAsync(shoppingListId, itemDto, currentUser.Id);
                await _bll.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding pantry item to shopping list: {Message}", ex.Message);
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAjax(UserPantryItemCreateDto pantryItemDto)
        {
            _logger.LogInformation(
                "CreateAjax method called with data: IngredientId={IngredientId}, Amount={Amount}, Unit={Unit}",
                pantryItemDto.IngredientId, pantryItemDto.AmountInStock, pantryItemDto.Unit);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state when creating pantry item via Ajax");
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return Json(new { success = false, message = "Validation failed: " + string.Join(", ", errors) });
            }

            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    _logger.LogWarning("User authentication failed when creating pantry item via Ajax");
                    return Json(new { success = false, message = "User authentication failed." });
                }

                // Map to BLL DTO
                var bllPantryItem = _mapper.Map<BLL.DTO.UserPantryItem>(pantryItemDto);

                // Add pantry item using BLL service
                var createdItem = await _bll.UserPantry.AddAsync(bllPantryItem, currentUser.Id);
                await _bll.SaveChangesAsync();

                _logger.LogInformation("Successfully saved pantry item via Ajax with ID: {Id}", createdItem.Id);

                // Get ingredient name for the response
                var ingredient = await _bll.Ingredients.FindAsync(createdItem.IngredientId);
                string ingredientName = ingredient?.Name ?? "Unknown Ingredient";

                return Json(new
                {
                    success = true,
                    message = "Item added to pantry successfully.",
                    id = createdItem.Id,
                    ingredientName = ingredientName
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating pantry item via Ajax: {Message}", ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}