using AutoMapper;
using MealPlanner.Models.Inventory;
using MealPlanner.Models.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MealPlanner.Controllers.Admin
{
    [Authorize(Roles = "Admin,AdminUsers")]
    [Route("Admin/Users")]  // FIXED: Updated route
    public class AdminUsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AdminUsersController> _logger;
        private readonly IMapper _mapper;

        public AdminUsersController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<AdminUsersController> logger,
            IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: Admin/Users
        [HttpGet("")]
        public async Task<IActionResult> Index(string searchTerm = "", string roleFilter = "", int page = 1, int pageSize = 10)
        {
            var query = _userManager.Users.AsQueryable();

            // Apply search filter
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(u => 
                    u.UserName!.Contains(searchTerm) || 
                    u.Email!.Contains(searchTerm) ||
                    u.FirstName!.Contains(searchTerm) ||
                    u.LastName!.Contains(searchTerm));
            }

            var totalUsers = query.Count();
            var users = query
                .OrderBy(u => u.UserName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var userViewModels = new List<UserManagementViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                
                // Apply role filter
                if (!string.IsNullOrEmpty(roleFilter) && !roles.Contains(roleFilter))
                    continue;

                userViewModels.Add(new UserManagementViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName ?? "Unknown",
                    Email = user.Email ?? "No Email",
                    FirstName = user.FirstName ?? "",
                    LastName = user.LastName ?? "",
                    EmailConfirmed = user.EmailConfirmed,
                    LockoutEnd = user.LockoutEnd,
                    Created = user.Created,
                    Roles = roles.ToList()
                });
            }

            var viewModel = new UsersIndexViewModel
            {
                Users = userViewModels,
                CurrentPage = page,
                PageSize = pageSize,
                TotalUsers = totalUsers,
                TotalPages = (int)Math.Ceiling((double)totalUsers / pageSize),
                SearchTerm = searchTerm,
                RoleFilter = roleFilter,
                AvailableRoles = _roleManager.Roles.Select(r => r.Name!).ToList()
            };

            return View(viewModel);
        }

        // GET: AdminUsers/Users/Details/5
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var claims = await _userManager.GetClaimsAsync(user);

            var viewModel = new UserDetailsViewModel
            {
                Id = user.Id,
                UserName = user.UserName ?? "Unknown",
                Email = user.Email ?? "No Email",
                FirstName = user.FirstName ?? "",
                LastName = user.LastName ?? "",
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                TwoFactorEnabled = user.TwoFactorEnabled,
                LockoutEnd = user.LockoutEnd,
                AccessFailedCount = user.AccessFailedCount,
                Created = user.Created,
                Roles = roles.ToList(),
                Claims = claims.Select(c => $"{c.Type}: {c.Value}").ToList()
            };

            return View(viewModel);
        }

        // GET: AdminUsers/Users/Edit/5
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = _roleManager.Roles.ToList();

            var viewModel = new EditUserRolesViewModel
            {
                UserId = user.Id,
                UserName = user.UserName ?? "Unknown",
                Email = user.Email ?? "No Email",
                UserRoles = userRoles.ToList(),
                AllRoles = allRoles.Select(r => new SelectListItem
                {
                    Value = r.Name,
                    Text = r.Name,
                    Selected = userRoles.Contains(r.Name!)
                }).ToList()
            };

            return View(viewModel);
        }

        // POST: AdminUsers/Users/Edit/5
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, EditUserRolesViewModel model)
        {
            if (id != model.UserId)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Get current roles
                    var currentRoles = await _userManager.GetRolesAsync(user);

                    // Remove all current roles
                    var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                    if (!removeResult.Succeeded)
                    {
                        foreach (var error in removeResult.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                        return View(model);
                    }

                    // Add selected roles
                    if (model.SelectedRoles != null && model.SelectedRoles.Any())
                    {
                        var addResult = await _userManager.AddToRolesAsync(user, model.SelectedRoles);
                        if (!addResult.Succeeded)
                        {
                            foreach (var error in addResult.Errors)
                            {
                                ModelState.AddModelError("", error.Description);
                            }
                            return View(model);
                        }
                    }

                    TempData["SuccessMessage"] = $"Roles updated successfully for {user.UserName}";
                    return RedirectToAction(nameof(Details), new { id });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating user roles for user {UserId}", id);
                    ModelState.AddModelError("", "An error occurred while updating user roles");
                }
            }

            // Repopulate dropdown on error
            var allRoles = _roleManager.Roles.ToList();
            model.AllRoles = allRoles.Select(r => new SelectListItem
            {
                Value = r.Name,
                Text = r.Name
            }).ToList();

            return View(model);
        }

        // POST: AdminUsers/Users/ToggleLockout/5
        [HttpPost("ToggleLockout/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleLockout(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            try
            {
                if (user.LockoutEnd == null || user.LockoutEnd <= DateTimeOffset.UtcNow)
                {
                    // Lock the user
                    await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100));
                    TempData["SuccessMessage"] = $"User {user.UserName} has been locked out";
                }
                else
                {
                    // Unlock the user
                    await _userManager.SetLockoutEndDateAsync(user, null);
                    TempData["SuccessMessage"] = $"User {user.UserName} has been unlocked";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling lockout for user {UserId}", id);
                TempData["ErrorMessage"] = "An error occurred while updating user lockout status";
            }

            return RedirectToAction(nameof(Details), new { id });
        }
    }
}