using Microsoft.AspNetCore.Identity;
using MealPlanner.Models.Inventory;

namespace MealPlanner.Services
{
    public class RoleSeederService : IRoleSeederService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RoleSeederService> _logger;
        private readonly IConfiguration _configuration;

        public RoleSeederService(
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            ILogger<RoleSeederService> logger,
            IConfiguration configuration)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task SeedRolesAsync()
        {
            string[] roles = { "Admin", "Moderator", "Premium", "User" };

            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                    _logger.LogInformation($"Created role: {role}");
                }
            }
        }

        public async Task CreateDefaultAdminAsync()
        {
            const string adminEmail = "admin@mealplanner.com";
            var adminPassword = _configuration["ADMIN_PASSWORD"];
            if (string.IsNullOrWhiteSpace(adminPassword))
            {
                _logger.LogWarning("ADMIN_PASSWORD is not set; default admin user was not created.");
                return;
            }

            var adminUser = await _userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = "admin",
                    Email = adminEmail,
                    FirstName = "System",
                    LastName = "Administrator",
                    EmailConfirmed = true,
                    Created = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(adminUser, "Admin");
                    _logger.LogInformation($"Created default admin user: {adminEmail}");
                }
                else
                {
                    _logger.LogError($"Failed to create admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
            else
            {
                // Ensure existing user has admin role
                if (!await _userManager.IsInRoleAsync(adminUser, "Admin"))
                {
                    await _userManager.AddToRoleAsync(adminUser, "Admin");
                    _logger.LogInformation($"Added Admin role to existing user: {adminEmail}");
                }
            }
        }
    }
}
