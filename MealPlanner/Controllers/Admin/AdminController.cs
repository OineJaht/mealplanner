using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MealPlanner.BLL.Services;
using MealPlanner.Models.Inventory;
using MealPlanner.Models.ViewModels.Admin;
using AutoMapper;
using System.Security.Claims;

namespace MealPlanner.Controllers.Admin
{
    [Authorize(Roles = "Admin,AdminUsers")] // Start with just [Authorize] to test basic authentication
    [Route("Admin")]
    public class AdminController : Controller
    {
        private readonly IAppBLL _bll;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AdminController> _logger;
        private readonly IMapper _mapper;

        public AdminController(
            IAppBLL bll,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<AdminController> logger,
            IMapper mapper)
        {
            _bll = bll;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: Admin (Default Dashboard)
        [HttpGet("")]
        [HttpGet("Dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            // Debug logging
            _logger.LogInformation("Dashboard accessed by user: {UserName}, ID: {UserId}", 
                User.Identity?.Name, User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Check if user is admin
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId))
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    _logger.LogInformation("User {UserName} has roles: {Roles}", user.UserName, string.Join(", ", roles));
                    
                    // For now, let's allow any authenticated user to access admin
                    // We'll add role checking back once we confirm authentication works
                    // if (!roles.Contains("Admin"))
                    // {
                    //     return Forbid();
                    // }
                }
            }

            var viewModel = new AdminDashboardViewModel();

            try
            {
                // User statistics
                var allUsers = _userManager.Users.ToList();
                viewModel.TotalUsers = allUsers.Count;
                viewModel.AdminUsers = 0;
                viewModel.ModeratorUsers = 0;
                viewModel.PremiumUsers = 0;
                viewModel.RegularUsers = 0;

                foreach (var userItem in allUsers)
                {
                    var roles = await _userManager.GetRolesAsync(userItem);
                    if (roles.Contains("Admin")) viewModel.AdminUsers++;
                    else if (roles.Contains("Moderator")) viewModel.ModeratorUsers++;
                    else if (roles.Contains("Premium")) viewModel.PremiumUsers++;
                    else viewModel.RegularUsers++;
                }

                // Content statistics
                var allRecipes = await _bll.Recipes.AllAsync();
                viewModel.TotalRecipes = allRecipes.Count;

                var allIngredients = await _bll.Ingredients.AllAsync();
                viewModel.TotalIngredients = allIngredients.Count;

                var allCategories = await _bll.Categories.AllAsync();
                viewModel.TotalCategories = allCategories.Count;

                // Recent activity - last 30 days
                var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
                viewModel.RecentUsers = allUsers.Count(u => u.Created >= thirtyDaysAgo);
                viewModel.RecentRecipes = allRecipes.Count(r => r.CreatedAt >= thirtyDaysAgo);

                // Recent users for display
                viewModel.LatestUsers = allUsers
                    .OrderByDescending(u => u.Created)
                    .Take(5)
                    .Select(u => new UserSummaryViewModel
                    {
                        Id = u.Id,
                        UserName = u.UserName ?? "Unknown",
                        Email = u.Email ?? "No Email",
                        FirstName = u.FirstName ?? "",
                        LastName = u.LastName ?? "",
                        Created = u.Created,
                        EmailConfirmed = u.EmailConfirmed
                    })
                    .ToList();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading admin dashboard");
                TempData["ErrorMessage"] = "Error loading dashboard data";
            }

            return View(viewModel);
        }

        [HttpGet("TestAuth")]
        public async Task<IActionResult> TestAuth()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = User.Identity?.Name;
            var isAuthenticated = User.Identity?.IsAuthenticated ?? false;

            ViewBag.Debug = new
            {
                IsAuthenticated = isAuthenticated,
                UserName = userName,
                UserId = userId,
                Claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList()
            };

            if (!string.IsNullOrEmpty(userId))
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    ViewBag.DatabaseUser = user;
                    ViewBag.UserRoles = await _userManager.GetRolesAsync(user);
                }
            }

            return View();
        }
        
        [HttpGet("AuthTest")]
        public IActionResult AuthTest()
        {
            return Json(new
            {
                IsAuthenticated = User.Identity?.IsAuthenticated ?? false,
                UserName = User.Identity?.Name,
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                IsInAdminRole = User.IsInRole("Admin"),
                IsInAdminUsersRole = User.IsInRole("AdminUsers"),
                AllRoles = User.Claims
                    .Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value)
                    .ToList()
            });
        }
    }
}