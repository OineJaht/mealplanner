// MealPlanner.DAL.DTO/BaseDto.cs
using System;

namespace MealPlanner.DAL.DTO
{
    /// <summary>
    /// Base DTO for entities with ID
    /// </summary>
    public abstract class BaseDto
    {
        public int Id { get; set; }
    }

    /// <summary>
    /// Base DTO for entities with user ownership (excludes AppUser navigation property)
    /// </summary>
    public abstract class BaseUserOwnedDto : BaseDto
    {
        public string UserId { get; set; } = string.Empty;
    }

    /// <summary>
    /// Recipe DTO for DAL layer
    /// </summary>
    public class RecipeDto : BaseUserOwnedDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int DefaultServings { get; set; }
        public int PrepTimeMinutes { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsFavorite { get; set; }
        public string? Instructions { get; set; }

        // Navigation properties as DTOs (no domain entities)
        public ICollection<RecipeCategoryDto> RecipeCategories { get; set; } = new List<RecipeCategoryDto>();
        public ICollection<RecipeIngredientDto> RecipeIngredients { get; set; } = new List<RecipeIngredientDto>();
        public ICollection<CookingStepDto> CookingSteps { get; set; } = new List<CookingStepDto>();
        public RecipeNutritionDto? RecipeNutrition { get; set; }
        public ICollection<MealPlanItemDto> MealPlanItems { get; set; } = new List<MealPlanItemDto>();
    }

    /// <summary>
    /// Recipe Category junction DTO
    /// </summary>
    public class RecipeCategoryDto : BaseDto
    {
        public int RecipeId { get; set; }
        public int CategoryId { get; set; }
        public CategoryDto? Category { get; set; }
    }

    /// <summary>
    /// Recipe Ingredient DTO
    /// </summary>
    public class RecipeIngredientDto : BaseDto
    {
        public int RecipeId { get; set; }
        public int IngredientId { get; set; }
        public decimal Amount { get; set; }
        public string Unit { get; set; } = string.Empty;
        public IngredientDto? Ingredient { get; set; }
    }

    /// <summary>
    /// Cooking Step DTO
    /// </summary>
    public class CookingStepDto : BaseDto
    {
        public int RecipeId { get; set; }
        public int StepNumber { get; set; }
        public string Instruction { get; set; } = string.Empty;
        public int TimingMinutes { get; set; }
    }

    /// <summary>
    /// Recipe Nutrition DTO
    /// </summary>
    public class RecipeNutritionDto : BaseDto
    {
        public int RecipeId { get; set; }
        public decimal TotalCalories { get; set; }
        public decimal TotalProteinG { get; set; }
        public decimal TotalCarbsG { get; set; }
        public decimal TotalFatG { get; set; }
        public decimal TotalFiberG { get; set; }
        public decimal TotalSugarG { get; set; }
        public decimal TotalSodiumG { get; set; }
        public decimal CaloriesPerServing { get; set; }
    }

    /// <summary>
    /// Category DTO
    /// </summary>
    public class CategoryDto : BaseDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public ICollection<RecipeCategoryDto> RecipeCategories { get; set; } = new List<RecipeCategoryDto>();
    }

    /// <summary>
    /// Ingredient DTO
    /// </summary>
    public class IngredientDto : BaseDto
    {
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public IngredientNutritionDto? IngredientNutrition { get; set; }
        public ICollection<RecipeIngredientDto> RecipeIngredients { get; set; } = new List<RecipeIngredientDto>();
        public ICollection<UserPantryItemDto> UserPantryItems { get; set; } = new List<UserPantryItemDto>();
        public ICollection<ShoppingListItemDto> ShoppingListItems { get; set; } = new List<ShoppingListItemDto>();
    }

    /// <summary>
    /// Ingredient Nutrition DTO
    /// </summary>
    public class IngredientNutritionDto : BaseDto
    {
        public int IngredientId { get; set; }
        public decimal CaloriesPer100g { get; set; }
        public decimal ProteinG { get; set; }
        public decimal CarbsG { get; set; }
        public decimal FatG { get; set; }
        public decimal FiberG { get; set; }
        public decimal SugarG { get; set; }
        public decimal SodiumMg { get; set; }
    }
}