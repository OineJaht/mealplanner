using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Localization;
using AutoMapper;
using MealPlanner.BLL.Services; // Changed from UnitOfWork

using MealPlanner.Dto;
using MealPlanner.Models.Inventory;
using MealPlanner.Models.ViewModels;
using Microsoft.Extensions.Localization;

namespace MealPlanner.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAppBLL _bll; // Changed from IUnitOfWork
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public HomeController(
            ILogger<HomeController> logger,
            IAppBLL bll, // Changed parameter
            UserManager<ApplicationUser> userManager,
            IMapper mapper)
        {
            _logger = logger;
            _bll = bll; // Changed assignment
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                // Get current user
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    return Challenge(); // Redirect to login if user record not found
                }

                var today = DateTime.UtcNow.Date;

                // Get current meal plan using BLL
                var currentMealPlan = await _bll.MealPlans.GetCurrentAsync(currentUser.Id);
                var currentMealPlanDto = currentMealPlan != null ? 
                    _mapper.Map<MealPlanDto>(currentMealPlan) : null;

                // Get recent recipes using BLL
                var recentRecipes = await _bll.Recipes.GetRecentAsync(currentUser.Id, 5);
                var recentRecipeDtos = _mapper.Map<List<RecipeDto>>(recentRecipes);

                // Get favorite recipes using BLL
                var favoriteRecipes = await _bll.Recipes.GetFavoritesAsync(currentUser.Id);
                var favoriteRecipeDtos = _mapper.Map<List<RecipeDto>>(favoriteRecipes);

                // Get expiring pantry items using BLL
                var expiringPantryItems = await _bll.UserPantry.GetExpiringItemsAsync(currentUser.Id, 7);
                var expiringPantryItemDtos = _mapper.Map<List<UserPantryItemDto>>(expiringPantryItems);

                // Get shopping lists using BLL
                var shoppingLists = await _bll.ShoppingLists.AllAsync(currentUser.Id);
                var shoppingListDtos = _mapper.Map<List<ShoppingListDto>>(shoppingLists);

                // Dashboard data
                var viewModel = new DashboardViewModel
                {
                    RecentRecipes = recentRecipeDtos,
                    FavoriteRecipes = favoriteRecipeDtos,
                    CurrentMealPlan = currentMealPlanDto,
                    ExpiringPantryItems = expiringPantryItemDtos,
                    ShoppingLists = shoppingListDtos
                };

                return View(viewModel);
            }

            // For non-authenticated users, show landing page
            return View("Landing");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }
        
        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            // Validate the culture to make sure it's one we support
            var supportedCultures = new[] { "en", "ru" };
            if (!supportedCultures.Contains(culture))
            {
                culture = "en";
            }
    
            // Set the culture cookie
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions 
                { 
                    Expires = DateTimeOffset.UtcNow.AddYears(1),
                    IsEssential = true,
                    HttpOnly = false,
                    SameSite = SameSiteMode.Lax
                }
            );
            
            return LocalRedirect(returnUrl);
        }
        
        public IActionResult DebugResources()
        {
            var result = new Dictionary<string, object>();
    
            // Get all resource names in the assembly
            var resourceNames = typeof(Program).Assembly.GetManifestResourceNames();
            result["ResourceNames"] = resourceNames;
    
            // Get current cultures
            result["CurrentCulture"] = System.Globalization.CultureInfo.CurrentCulture.Name;
            result["CurrentUICulture"] = System.Globalization.CultureInfo.CurrentUICulture.Name;
    
            // Try to get localized values manually
            var localizerFactory = HttpContext.RequestServices.GetService<IStringLocalizerFactory>();
            if (localizerFactory != null)
            {
                var localizer = localizerFactory.Create(typeof(MealPlanner.Resources.SharedResource));
                result["MealPlannerValue"] = localizer["MealPlanner"].Value;
                result["IngredientsValue"] = localizer["Ingredients"].Value;
            }
    
            return Json(result);
        }
    }

    // DashboardViewModel - no changes needed
    public class DashboardViewModel
    {
        public IEnumerable<RecipeDto> RecentRecipes { get; set; } = Enumerable.Empty<RecipeDto>();
        public IEnumerable<RecipeDto> FavoriteRecipes { get; set; } = Enumerable.Empty<RecipeDto>();
        public MealPlanDto? CurrentMealPlan { get; set; }
        public IEnumerable<UserPantryItemDto> ExpiringPantryItems { get; set; } = Enumerable.Empty<UserPantryItemDto>();
        public IEnumerable<ShoppingListDto> ShoppingLists { get; set; } = Enumerable.Empty<ShoppingListDto>();
    }
}