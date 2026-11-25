// Models/ViewModels/Admin/AdminViewModels.cs
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MealPlanner.Models.ViewModels.Admin
{
    public class AdminDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int AdminUsers { get; set; }
        public int ModeratorUsers { get; set; }
        public int PremiumUsers { get; set; }
        public int RegularUsers { get; set; }
        
        public int TotalRecipes { get; set; }
        public int TotalIngredients { get; set; }
        public int TotalCategories { get; set; }
        
        public int RecentUsers { get; set; }
        public int RecentRecipes { get; set; }
        
        public List<UserSummaryViewModel> LatestUsers { get; set; } = new List<UserSummaryViewModel>();
    }

    public class UserSummaryViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime Created { get; set; }
        public bool EmailConfirmed { get; set; }
    }

    public class SystemInfoViewModel
    {
        public DateTime ServerTime { get; set; }
        public string ApplicationVersion { get; set; } = string.Empty;
        public string DotNetVersion { get; set; } = string.Empty;
        public string MachineName { get; set; } = string.Empty;
        public int ProcessorCount { get; set; }
        public long WorkingSet { get; set; }
        public string OSVersion { get; set; } = string.Empty;
    }

    public class UsersIndexViewModel
    {
        public List<UserManagementViewModel> Users { get; set; } = new List<UserManagementViewModel>();
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalUsers { get; set; }
        public int TotalPages { get; set; }
        public string SearchTerm { get; set; } = string.Empty;
        public string RoleFilter { get; set; } = string.Empty;
        public List<string> AvailableRoles { get; set; } = new List<string>();
    }

    public class UserManagementViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public bool EmailConfirmed { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public DateTime Created { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        
        public bool IsLockedOut => LockoutEnd.HasValue && LockoutEnd > DateTimeOffset.UtcNow;
    }

    public class UserDetailsViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public bool EmailConfirmed { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public int AccessFailedCount { get; set; }
        public DateTime Created { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public List<string> Claims { get; set; } = new List<string>();
        
        public bool IsLockedOut => LockoutEnd.HasValue && LockoutEnd > DateTimeOffset.UtcNow;
    }

    public class EditUserRolesViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<string> UserRoles { get; set; } = new List<string>();
        public List<SelectListItem> AllRoles { get; set; } = new List<SelectListItem>();
        public List<string> SelectedRoles { get; set; } = new List<string>();
    }

    public class ContentManagementViewModel
    {
        public int TotalRecipes { get; set; }
        public int TotalIngredients { get; set; }
        public int TotalCategories { get; set; }
        public List<RecipeManagementViewModel> RecentRecipes { get; set; } = new List<RecipeManagementViewModel>();
    }

    public class RecipeManagementViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public bool IsFavorite { get; set; }
        public int DefaultServings { get; set; }
        public int PrepTimeMinutes { get; set; }
    }

    public class RecipesManagementViewModel
    {
        public List<RecipeManagementViewModel> Recipes { get; set; } = new List<RecipeManagementViewModel>();
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalRecipes { get; set; }
        public int TotalPages { get; set; }
        public string SearchTerm { get; set; } = string.Empty;
    }

    public class AdminRecipeDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Instructions { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public bool IsFavorite { get; set; }
        public int DefaultServings { get; set; }
        public int PrepTimeMinutes { get; set; }
        public string? ImageUrl { get; set; }
        public List<string> Categories { get; set; } = new List<string>();
        public List<AdminRecipeIngredientViewModel> Ingredients { get; set; } = new List<AdminRecipeIngredientViewModel>();
        public List<AdminCookingStepViewModel> CookingSteps { get; set; } = new List<AdminCookingStepViewModel>();
        public AdminRecipeNutritionViewModel? Nutrition { get; set; }
    }

    public class AdminRecipeIngredientViewModel
    {
        public string IngredientName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Unit { get; set; } = string.Empty;
    }

    public class AdminCookingStepViewModel
    {
        public int StepNumber { get; set; }
        public string Instruction { get; set; } = string.Empty;
        public int TimingMinutes { get; set; }
    }

    public class AdminRecipeNutritionViewModel
    {
        public decimal TotalCalories { get; set; }
        public decimal TotalProteinG { get; set; }
        public decimal TotalCarbsG { get; set; }
        public decimal TotalFatG { get; set; }
        public decimal CaloriesPerServing { get; set; }
    }

    public class IngredientsManagementViewModel
    {
        public List<IngredientManagementViewModel> Ingredients { get; set; } = new List<IngredientManagementViewModel>();
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalIngredients { get; set; }
        public int TotalPages { get; set; }
        public string SearchTerm { get; set; } = string.Empty;
    }

    public class IngredientManagementViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
    }
}