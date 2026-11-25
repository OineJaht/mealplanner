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
using MealPlanner.Models.ViewModels.ShoppingLists;

namespace MealPlanner.Controllers
{
    [Authorize(AuthenticationSchemes = "AuthScheme")]
    public class ShoppingListsController : Controller
    {
        private readonly IAppBLL _bll;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ShoppingListsController> _logger;
        private readonly IMapper _mapper;

        public ShoppingListsController(
            IAppBLL bll,
            UserManager<ApplicationUser> userManager,
            ILogger<ShoppingListsController> logger,
            IMapper mapper)
        {
            _bll = bll;
            _userManager = userManager;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: ShoppingLists
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            var lists = await _bll.ShoppingLists.AllAsync(currentUser.Id);
            var totalItems = new Dictionary<int, int>();
            var purchasedItems = new Dictionary<int, int>();
            var topItems = new Dictionary<int, List<ShoppingListItemDto>>();

            foreach (var list in lists)
            {
                var itemCount = list.Items?.Count ?? 0;
                var purchasedCount = list.Items?.Count(i => i.Purchased) ?? 0;
                totalItems[list.Id] = itemCount;
                purchasedItems[list.Id] = purchasedCount;

                // Add TopItems population
                if (list.Items != null && list.Items.Any())
                {
                    var topItemsList = list.Items.Take(3).ToList();
                    topItems[list.Id] = _mapper.Map<List<ShoppingListItemDto>>(topItemsList);
                }
            }

            var viewModel = new ShoppingListIndexViewModel
            {
                Lists = _mapper.Map<List<ShoppingListDto>>(lists),
                TotalItemsPerList = totalItems,
                PurchasedItemsPerList = purchasedItems,
                TopItems = topItems
            };

            return View(viewModel);
        }

        // GET: ShoppingLists/Details/5
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

            // Get shopping list with items using BLL service
            var shoppingList = await _bll.ShoppingLists.GetWithItemsAsync(id.Value, currentUser.Id);

            if (shoppingList == null)
            {
                return NotFound();
            }

            // Populate the ingredient dropdown for the Add Item modal
            var ingredients = await _bll.Ingredients.AllAsync();

            var ingredientList = ingredients
                .Select(i => new IngredientListItem
                {
                    Id = i.Id,
                    Name = i.Name
                }).ToList();

            var ingredientCategories = ingredients
                .ToDictionary(i => i.Id, i => i.Category ?? "Unknown");

            var viewModel = new ShoppingListDetailViewModel
            {
                ShoppingList = _mapper.Map<ShoppingListDetailDto>(shoppingList),
                AvailableIngredients = new SelectList(ingredients.OrderBy(i => i.Name), "Id", "Name"),
                NewItem = new ShoppingListItemCreateDto { ShoppingListId = id.Value },
                Ingredients = ingredientList,
                IngredientCategories = ingredientCategories
            };

            // DEBUG: ModelState validation
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState is invalid in Details action:");
                foreach (var modelError in ModelState)
                {
                    var key = modelError.Key;
                    var errors = modelError.Value.Errors;

                    _logger.LogWarning($"Key: {key}");
                    foreach (var error in errors)
                    {
                        _logger.LogWarning($"  Error: {error.ErrorMessage}");
                        if (error.Exception != null)
                        {
                            _logger.LogWarning($"  Exception: {error.Exception.Message}");
                        }
                    }
                }
            }
            else
            {
                _logger.LogInformation("ModelState is valid in Details action");
            }

            // DEBUG: View model validation
            _logger.LogInformation($"ShoppingList: {(viewModel.ShoppingList != null ? "Not null" : "NULL")}");
            _logger.LogInformation($"ShoppingList Items Count: {viewModel.ShoppingList?.Items?.Count ?? 0}");
            _logger.LogInformation($"Available Ingredients Count: {ingredients.Count()}");
            _logger.LogInformation($"NewItem ShoppingListId: {viewModel.NewItem.ShoppingListId}");

            return View(viewModel);
        }

        // GET: ShoppingLists/Create
        public async Task<IActionResult> Create()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Challenge();

            return View(new ShoppingListCreateDto());
        }


        // POST: ShoppingLists/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ShoppingListCreateDto shoppingListDto)
        {
            if (!ModelState.IsValid) return View(shoppingListDto);

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Challenge();

            try
            {
                var shoppingList = _mapper.Map<BLL.DTO.ShoppingList>(shoppingListDto);
                shoppingList.UserId = currentUser.Id;

                await _bll.ShoppingLists.AddAsync(shoppingList, currentUser.Id);
                await _bll.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating shopping list: {Message}", ex.Message);
                ModelState.AddModelError("", "An error occurred while creating the shopping list: " + ex.Message);
                return View(shoppingListDto);
            }
        }


        // GET: ShoppingLists/Edit/5
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

            // Get shopping list with items using BLL service
            var shoppingList = await _bll.ShoppingLists.GetWithItemsAsync(id.Value, currentUser.Id);

            if (shoppingList == null)
            {
                return NotFound();
            }

            // Populate the ingredient dropdown for the Add Item modal
            var ingredients = await _bll.Ingredients.AllAsync();

            var viewModel = new ShoppingListEditViewModel
            {
                ShoppingList = _mapper.Map<ShoppingListUpdateDto>(shoppingList),
                AvailableIngredients = new SelectList(ingredients.OrderBy(i => i.Name), "Id", "Name"),
                NewItem = new ShoppingListItemCreateDto { ShoppingListId = id.Value }
            };

            return View(viewModel);
        }

        // POST: ShoppingLists/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ShoppingListEditViewModel viewModel)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            if (id != viewModel.ShoppingList.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var bllDto = _mapper.Map<BLL.DTO.ShoppingList>(viewModel.ShoppingList);

                    await _bll.ShoppingLists.UpdateAsync(bllDto, currentUser.Id);
                    await _bll.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating shopping list");
                    ModelState.AddModelError("", "An error occurred while updating the list.");
                }
            }

            // Repopulate dropdowns on error
            var ingredients = await _bll.Ingredients.AllAsync();
            viewModel.AvailableIngredients = new SelectList(ingredients, "Id", "Name");
            return View(viewModel);
        }


        // GET: ShoppingLists/Delete/5
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

            // Get shopping list with items using BLL service
            var shoppingList = await _bll.ShoppingLists.GetWithItemsAsync(id.Value, currentUser.Id);

            if (shoppingList == null)
            {
                return NotFound();
            }

            var shoppingListDto = _mapper.Map<ShoppingListDetailDto>(shoppingList);
            return View(shoppingListDto);
        }

        // POST: ShoppingLists/Delete/5
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
                // Delete shopping list using BLL service
                await _bll.ShoppingLists.RemoveAsync(id, currentUser.Id);
                await _bll.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting shopping list: {Message}", ex.Message);
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: ShoppingLists/CreateFromMealPlan
        public async Task<IActionResult> CreateFromMealPlan()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            // Get user's meal plans for dropdown using BLL service
            var userMealPlans = await _bll.MealPlans.AllAsync(currentUser.Id);

            var viewModel = new CreateFromMealPlanViewModel
            {
                AvailableMealPlans = new SelectList(userMealPlans, "Id", "Name")
            };

            return View(viewModel);
        }

        // POST: ShoppingLists/CreateFromMealPlan
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFromMealPlan(CreateFromMealPlanViewModel viewModel)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            if (string.IsNullOrEmpty(viewModel.ListName))
            {
                ModelState.AddModelError("ListName", "Shopping list name is required.");

                var userMealPlans = await _bll.MealPlans.AllAsync(currentUser.Id);
                viewModel.AvailableMealPlans = new SelectList(userMealPlans, "Id", "Name", viewModel.MealPlanId);

                return View(viewModel);
            }

            try
            {
                // Create shopping list from meal plan using BLL service
                var shoppingList = await _bll.ShoppingLists.CreateFromMealPlanAsync(
                    viewModel.MealPlanId, viewModel.ListName, currentUser.Id);
                await _bll.SaveChangesAsync();

                return RedirectToAction(nameof(Details), new { id = shoppingList.Id });
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);

                var userMealPlans = await _bll.MealPlans.AllAsync(currentUser.Id);
                viewModel.AvailableMealPlans = new SelectList(userMealPlans, "Id", "Name", viewModel.MealPlanId);

                return View(viewModel);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating shopping list from meal plan: {Message}", ex.Message);
                ModelState.AddModelError("", "An error occurred while creating the shopping list: " + ex.Message);

                var userMealPlans = await _bll.MealPlans.AllAsync(currentUser.Id);
                viewModel.AvailableMealPlans = new SelectList(userMealPlans, "Id", "Name", viewModel.MealPlanId);

                return View(viewModel);
            }
        }

        // POST: ShoppingLists/AddItem
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddItem(int shoppingListId, int ingredientId, decimal amount)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            try
            {
                // Create shopping list item DTO
                var itemDto = new ShoppingListItemCreateDto
                {
                    ShoppingListId = shoppingListId,
                    IngredientId = ingredientId,
                    Amount = amount,
                    Purchased = false
                };

                // Map to BLL DTO
                var bllItem = _mapper.Map<BLL.DTO.ShoppingListItem>(itemDto);

                // Add item using BLL service
                var addedItem = await _bll.ShoppingLists.AddItemAsync(shoppingListId, bllItem, currentUser.Id);
                await _bll.SaveChangesAsync();

                return RedirectToAction(nameof(Details), new { id = shoppingListId });
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding shopping list item: {Message}", ex.Message);
                return RedirectToAction(nameof(Details), new { id = shoppingListId });
            }
        }

        // POST: ShoppingLists/ToggleItem
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleItem(int itemId, bool purchased)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            try
            {
                // Toggle item using BLL service
                var updatedItem = await _bll.ShoppingLists.ToggleItemAsync(itemId, purchased, currentUser.Id);
                await _bll.SaveChangesAsync();

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    // Get all items for this shopping list to calculate counts
                    var shoppingList =
                        await _bll.ShoppingLists.GetWithItemsAsync(updatedItem.ShoppingListId, currentUser.Id);

                    // Check if shoppingList and its Items collection are not null before accessing Count
                    int totalCount = 0;
                    int purchasedCount = 0;

                    if (shoppingList != null && shoppingList.Items != null)
                    {
                        totalCount = shoppingList.Items.Count;
                        purchasedCount = shoppingList.Items.Count(i => i.Purchased);
                    }

                    return Json(new { success = true, purchasedCount, totalCount });
                }

                return RedirectToAction(nameof(Details), new { id = updatedItem.ShoppingListId });
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling shopping list item: {Message}", ex.Message);

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = ex.Message });
                }

                // In case of error, we need to know the shopping list ID to redirect
                // This might not be available if we couldn't get the item
                // So redirect to the index in that case
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: ShoppingLists/RemoveItem
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveItem(int itemId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Challenge();

            try
            {
                var shoppingListId = await _bll.ShoppingLists.GetOwnerShoppingListIdAsync(itemId, currentUser.Id);
                if (shoppingListId == null)
                    return Forbid();

                await _bll.ShoppingLists.RemoveItemAsync(itemId, currentUser.Id);
                await _bll.SaveChangesAsync();

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { success = true });

                return RedirectToAction(nameof(Details), new { id = shoppingListId.Value });
            }
            catch (ArgumentException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing shopping list item");
                return Json(new { success = false, message = ex.Message });
            }
        }


        // POST: ShoppingLists/RemoveCompletedItems
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveCompletedItems(int shoppingListId, int[] itemIds)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            try
            {
                // Remove completed items using BLL service
                // This might be implemented as a dedicated method like:
                var removedCount = await _bll.ShoppingLists.RemoveCompletedItemsAsync(shoppingListId, currentUser.Id);
                await _bll.SaveChangesAsync();

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    // This is an AJAX request - return a JSON result
                    return Json(new { success = true, removedCount });
                }

                return RedirectToAction(nameof(Details), new { id = shoppingListId });
            }
            catch (ArgumentException ex)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = ex.Message });
                }

                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Not authorized" });
                }

                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing completed items: {Message}", ex.Message);

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = ex.Message });
                }

                return RedirectToAction(nameof(Details), new { id = shoppingListId });
            }
        }

        // POST: ShoppingLists/UpdateItemAmount
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateItemAmount(int itemId, decimal amount)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Challenge();

            try
            {
                var itemInfo = await _bll.ShoppingLists.GetItemOwnerInfoAsync(itemId, currentUser.Id);
                if (itemInfo == null)
                    return Forbid();

                var shoppingListId = itemInfo.Value.shoppingListId;
                var purchased = itemInfo.Value.purchased;


                if (amount <= 0)
                {
                    await _bll.ShoppingLists.RemoveItemAsync(itemId, currentUser.Id);
                }
                else
                {
                    var dto = new ShoppingListItemUpdateDto
                    {
                        Id = itemId,
                        Amount = amount,
                        Purchased = purchased
                    };

                    var bllItem = _mapper.Map<BLL.DTO.ShoppingListItem>(dto);
                    await _bll.ShoppingLists.UpdateItemAsync(bllItem, currentUser.Id);
                }

                await _bll.SaveChangesAsync();

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { success = true });

                return RedirectToAction(nameof(Details), new { id = shoppingListId });
            }
            catch (ArgumentException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating item amount");
                return Json(new { success = false, message = ex.Message });
            }
        }


        // POST: ShoppingLists/CreateFromMealPlanAjax
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFromMealPlanAjax(int mealPlanId, string listName)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Json(new { success = false, message = "User not authenticated" });
            }

            if (string.IsNullOrEmpty(listName))
            {
                return Json(new { success = false, message = "Shopping list name is required" });
            }

            try
            {
                // Create shopping list from meal plan using BLL service
                var shoppingList =
                    await _bll.ShoppingLists.CreateFromMealPlanAsync(mealPlanId, listName, currentUser.Id);
                await _bll.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    id = shoppingList.Id,
                    name = shoppingList.Name,
                    itemCount = shoppingList.Items?.Count ?? 0,
                    redirectUrl = Url.Action("Details", new { id = shoppingList.Id })
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
                _logger.LogError(ex, "Error creating shopping list from meal plan via AJAX: {Message}", ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}