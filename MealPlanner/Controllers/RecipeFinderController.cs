using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AutoMapper;
using MealPlanner.BLL.Services;
using MealPlanner.Dto;
using MealPlanner.Models.Inventory;

namespace MealPlanner.Controllers
{
    [Authorize(AuthenticationSchemes = "AuthScheme")]
    public class RecipeFinderController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RecipeFinderController> _logger;
        private readonly IMapper _mapper;
        private readonly IAppBLL _bll;

        public RecipeFinderController(
            UserManager<ApplicationUser> userManager,
            ILogger<RecipeFinderController> logger,
            IMapper mapper,
            IAppBLL bll)
        {
            _userManager = userManager;
            _logger = logger;
            _mapper = mapper;
            _bll = bll;
        }

        // GET: RecipeFinder
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Challenge();

            // Get pantry ingredients using BLL service
            var pantryIngredients = await _bll.UserPantry.GetUserIngredientsAsync(currentUser.Id);

            // Get all ingredients using BLL service
            var allIngredients = await _bll.Ingredients.AllAsync();

            // Create BLL DTO view model
            var viewModel = new BLL.DTO.RecipeFinderViewModel
            {
                PantryIngredients = pantryIngredients,
                AllIngredients = allIngredients,
                SelectedIngredientIds = pantryIngredients.Select(i => i.Id).ToList()
            };

            return View(viewModel);
        }

        // POST: RecipeFinder/AddMissingIngredientsToShoppingList
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMissingIngredientsToShoppingList(int recipeId,
            List<int> availableIngredientIds, int? shoppingListId = null)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            // Get the recipe with ingredients using BLL
            var recipe = await _bll.Recipes.GetWithDetailsAsync(recipeId, currentUser.Id);
            if (recipe == null)
            {
                return NotFound();
            }

            // Get missing ingredients
            var recipeIngredientIds = recipe.Ingredients.Select(ri => ri.IngredientId).ToList();
            var missingIngredientIds = recipeIngredientIds.Except(availableIngredientIds ?? new List<int>()).ToList();

            if (!missingIngredientIds.Any())
            {
                return RedirectToAction("RecipeDetails", new { id = recipeId, availableIngredientIds });
            }

            // Determine which shopping list to use
            BLL.DTO.ShoppingList targetShoppingList;

            if (shoppingListId.HasValue)
            {
                var foundList = await _bll.ShoppingLists.FindAsync(shoppingListId.Value, currentUser.Id);
                if (foundList != null)
                {
                    targetShoppingList = foundList;
                }
                else
                {
                    // Create new if provided ID is invalid
                    var newListDto = new BLL.DTO.ShoppingList
                    {
                        Name = $"Missing ingredients for {recipe.Name}",
                        CreatedDate = DateTime.UtcNow
                    };
                    targetShoppingList = await _bll.ShoppingLists.AddAsync(newListDto, currentUser.Id);
                }
            }
            else
            {
                // Create new shopping list
                var newListDto = new BLL.DTO.ShoppingList
                {
                    Name = $"Missing ingredients for {recipe.Name}",
                    CreatedDate = DateTime.UtcNow
                };
                targetShoppingList = await _bll.ShoppingLists.AddAsync(newListDto, currentUser.Id);
            }

            // Add missing ingredients to shopping list
            foreach (var recipeIngredient in recipe.Ingredients.Where(ri =>
                         missingIngredientIds.Contains(ri.IngredientId)))
            {
                var shoppingListItemDto = new BLL.DTO.ShoppingListItem
                {
                    IngredientId = recipeIngredient.IngredientId,
                    Amount = recipeIngredient.Amount,
                    Purchased = false
                };

                await _bll.ShoppingLists.AddItemAsync(targetShoppingList.Id, shoppingListItemDto, currentUser.Id);
            }

            await _bll.SaveChangesAsync();

            // Redirect to shopping list details
            return RedirectToAction("Details", "ShoppingLists", new { id = targetShoppingList.Id });
        }

        // POST: RecipeFinder/FindRecipes
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FindRecipes(List<int> ingredientIds, bool matchAll = false)
        {
            if (ingredientIds == null || !ingredientIds.Any())
            {
                return RedirectToAction(nameof(Index));
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            // Use BLL service to find recipes - much simpler!
            var matches = await _bll.RecipeFinder.FindRecipesAsync(currentUser.Id, ingredientIds, matchAll);
    
            var selectedIngredients = await _bll.Ingredients.AllAsync();
            var filteredIngredients = selectedIngredients.Where(i => ingredientIds.Contains(i.Id)).ToList();

            var resultViewModel = new FindRecipesResultViewModel
            {
                Matches = matches,
                SelectedIngredients = filteredIngredients,
                MatchAll = matchAll
            };

            return View(resultViewModel);
        }

        // GET: RecipeFinder/RecipeDetails/5
        public async Task<IActionResult> RecipeDetails(int id, List<int> availableIngredientIds)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            // Use BLL service to get recipe with availability information
            var recipeWithAvailability = await _bll.RecipeFinder.GetRecipeWithAvailabilityAsync(
                id, availableIngredientIds, currentUser.Id);

            if (recipeWithAvailability == null)
            {
                return NotFound();
            }

            return View(recipeWithAvailability);
        }
    }
}