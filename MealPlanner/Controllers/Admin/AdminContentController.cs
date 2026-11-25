// Controllers/AdminUsers/AdminContentController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MealPlanner.BLL.Services;
using MealPlanner.Models.Inventory;
using MealPlanner.Models.ViewModels.Admin;
using AutoMapper;

namespace MealPlanner.Controllers.Admin
{
    [Authorize(Roles = "Admin,AdminUsers")]
    [Route("Admin/Content")]  // FIXED: Updated route
    public class AdminContentController : Controller
    {
        private readonly IAppBLL _bll;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AdminContentController> _logger;
        private readonly IMapper _mapper;

        public AdminContentController(
            IAppBLL bll,
            UserManager<ApplicationUser> userManager,
            ILogger<AdminContentController> logger,
            IMapper mapper)
        {
            _bll = bll;
            _userManager = userManager;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: Admin/Content
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var recipes = await _bll.Recipes.AllAsync();
                var ingredients = await _bll.Ingredients.AllAsync();
                var categories = await _bll.Categories.AllAsync();

                var viewModel = new ContentManagementViewModel
                {
                    TotalRecipes = recipes.Count,
                    TotalIngredients = ingredients.Count,
                    TotalCategories = categories.Count,
                    RecentRecipes = recipes
                        .OrderByDescending(r => r.CreatedAt)
                        .Take(10)
                        .Select(r => new RecipeManagementViewModel
                        {
                            Id = r.Id,
                            Name = r.Name,
                            CreatedAt = r.CreatedAt,
                            UserId = r.UserId,
                            IsFavorite = r.IsFavorite,
                            DefaultServings = r.DefaultServings
                        }).ToList()
                };

                // Get usernames for recent recipes
                foreach (var recipe in viewModel.RecentRecipes)
                {
                    var user = await _userManager.FindByIdAsync(recipe.UserId);
                    recipe.UserName = user?.UserName ?? "Unknown User";
                }

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading content management");
                TempData["ErrorMessage"] = "Error loading content data";
                return View(new ContentManagementViewModel());
            }
        }

        // GET: AdminUsers/Content/Recipes
        [HttpGet("Recipes")]
        public async Task<IActionResult> Recipes(string searchTerm = "", int page = 1, int pageSize = 20)
        {
            try
            {
                var allRecipes = await _bll.Recipes.AllAsync();
                
                // Apply search filter
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    allRecipes = allRecipes.Where(r => 
                        r.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        (r.Description != null && r.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)))
                        .ToList();
                }

                var totalRecipes = allRecipes.Count;
                var recipes = allRecipes
                    .OrderByDescending(r => r.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var recipeViewModels = new List<RecipeManagementViewModel>();

                foreach (var recipe in recipes)
                {
                    var user = await _userManager.FindByIdAsync(recipe.UserId);
                    recipeViewModels.Add(new RecipeManagementViewModel
                    {
                        Id = recipe.Id,
                        Name = recipe.Name,
                        Description = recipe.Description,
                        CreatedAt = recipe.CreatedAt,
                        UserId = recipe.UserId,
                        UserName = user?.UserName ?? "Unknown User",
                        IsFavorite = recipe.IsFavorite,
                        DefaultServings = recipe.DefaultServings,
                        PrepTimeMinutes = recipe.PrepTimeMinutes
                    });
                }

                var viewModel = new RecipesManagementViewModel
                {
                    Recipes = recipeViewModels,
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalRecipes = totalRecipes,
                    TotalPages = (int)Math.Ceiling((double)totalRecipes / pageSize),
                    SearchTerm = searchTerm
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading recipes management");
                TempData["ErrorMessage"] = "Error loading recipes data";
                return View(new RecipesManagementViewModel());
            }
        }

        // GET: AdminUsers/Content/Recipe/5
        [HttpGet("Recipe/{id}")]
        public async Task<IActionResult> RecipeDetails(int id)
        {
            try
            {
                var recipe = await _bll.Recipes.GetWithDetailsAsync(id);
                if (recipe == null)
                {
                    return NotFound();
                }

                var user = await _userManager.FindByIdAsync(recipe.UserId);

                var viewModel = new AdminRecipeDetailsViewModel
                {
                    Id = recipe.Id,
                    Name = recipe.Name,
                    Description = recipe.Description,
                    Instructions = recipe.Instructions,
                    CreatedAt = recipe.CreatedAt,
                    UserId = recipe.UserId,
                    UserName = user?.UserName ?? "Unknown User",
                    UserEmail = user?.Email ?? "No Email",
                    IsFavorite = recipe.IsFavorite,
                    DefaultServings = recipe.DefaultServings,
                    PrepTimeMinutes = recipe.PrepTimeMinutes,
                    ImageUrl = recipe.ImageUrl,
                    Categories = recipe.Categories.Select(c => c.Name).ToList(),
                    Ingredients = recipe.Ingredients.Select(ri => new AdminRecipeIngredientViewModel
                    {
                        IngredientName = ri.IngredientName,
                        Amount = ri.Amount,
                        Unit = ri.Unit
                    }).ToList(),
                    CookingSteps = recipe.CookingSteps.Select(cs => new AdminCookingStepViewModel
                    {
                        StepNumber = cs.StepNumber,
                        Instruction = cs.Instruction,
                        TimingMinutes = cs.TimingMinutes
                    }).ToList(),
                    Nutrition = recipe.Nutrition != null ? new AdminRecipeNutritionViewModel
                    {
                        TotalCalories = recipe.Nutrition.TotalCalories,
                        TotalProteinG = recipe.Nutrition.TotalProteinG,
                        TotalCarbsG = recipe.Nutrition.TotalCarbsG,
                        TotalFatG = recipe.Nutrition.TotalFatG,
                        CaloriesPerServing = recipe.Nutrition.CaloriesPerServing
                    } : null
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading recipe details for recipe {RecipeId}", id);
                TempData["ErrorMessage"] = "Error loading recipe details";
                return RedirectToAction(nameof(Recipes));
            }
        }

        // POST: AdminUsers/Content/Recipe/Delete/5
        [HttpPost("Recipe/Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRecipe(int id)
        {
            try
            {
                var recipe = await _bll.Recipes.FindAsync(id);
                if (recipe == null)
                {
                    return NotFound();
                }

                // Get admin user for deletion
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    return Challenge();
                }

                await _bll.Recipes.RemoveAsync(id, currentUser.Id);
                await _bll.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Recipe '{recipe.Name}' has been deleted";
                _logger.LogInformation("AdminUsers {AdminId} deleted recipe {RecipeId}", currentUser.Id, id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting recipe {RecipeId}", id);
                TempData["ErrorMessage"] = "Error deleting recipe";
            }

            return RedirectToAction(nameof(Recipes));
        }

        // GET: AdminUsers/Content/Ingredients
        [HttpGet("Ingredients")]
        public async Task<IActionResult> Ingredients(string searchTerm = "", int page = 1, int pageSize = 20)
        {
            try
            {
                var allIngredients = await _bll.Ingredients.AllAsync();
                
                // Apply search filter
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    allIngredients = allIngredients.Where(i => 
                        i.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        i.Category.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                var totalIngredients = allIngredients.Count;
                var ingredients = allIngredients
                    .OrderBy(i => i.Name)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var viewModel = new IngredientsManagementViewModel
                {
                    Ingredients = ingredients.Select(i => new IngredientManagementViewModel
                    {
                        Id = i.Id,
                        Name = i.Name,
                        Category = i.Category
                    }).ToList(),
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalIngredients = totalIngredients,
                    TotalPages = (int)Math.Ceiling((double)totalIngredients / pageSize),
                    SearchTerm = searchTerm
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading ingredients management");
                TempData["ErrorMessage"] = "Error loading ingredients data";
                return View(new IngredientsManagementViewModel());
            }
        }
    }
}