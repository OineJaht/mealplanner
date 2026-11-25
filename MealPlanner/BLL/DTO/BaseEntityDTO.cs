// MealPlanner.BLL/DTO/BaseEntityDTO.cs
using System;
using MealPlanner.Dto;

namespace MealPlanner.BLL.DTO
{
    /// <summary>
    /// Base class for all DTO entities
    /// </summary>
    public abstract class BaseEntityDTO
    {
        /// <summary>
        /// Entity ID
        /// </summary>
        public int Id { get; set; }
    }
    
    /// <summary>
    /// Shopping list item DTO - alternative name to avoid conflicts
    /// </summary>
    public class ShoppingListItemDto : BaseEntityDTO
    {
        /// <summary>
        /// Shopping list ID
        /// </summary>
        public int ShoppingListId { get; set; }

        /// <summary>
        /// Ingredient ID
        /// </summary>
        public int IngredientId { get; set; }

        /// <summary>
        /// Ingredient name
        /// </summary>
        public string IngredientName { get; set; } = string.Empty;

        /// <summary>
        /// Amount of ingredient
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Is item purchased
        /// </summary>
        public bool Purchased { get; set; }

        /// <summary>
        /// Unit of measurement
        /// </summary>
        public string Unit { get; set; } = string.Empty;
    }
    
    /// <summary>
    /// Recipe DTO
    /// </summary>
    public class Recipe : BaseEntityDTO
    {
        /// <summary>
        /// User ID who owns this recipe
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Recipe name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Recipe description
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Default number of servings
        /// </summary>
        public int DefaultServings { get; set; }

        /// <summary>
        /// Preparation time in minutes
        /// </summary>
        public int PrepTimeMinutes { get; set; }

        /// <summary>
        /// Date recipe was created
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Recipe image URL
        /// </summary>
        public string? ImageUrl { get; set; }

        /// <summary>
        /// Is recipe marked as favorite
        /// </summary>
        public bool IsFavorite { get; set; }

        /// <summary>
        /// Recipe categories
        /// </summary>
        public List<Category> Categories { get; set; } = new List<Category>();
    }

    /// <summary>
    /// Detailed recipe with ingredients, steps and nutrition
    /// </summary>
    public class RecipeDetail : Recipe
    {
        /// <summary>
        /// Recipe instructions
        /// </summary>
        public string Instructions { get; set; } = string.Empty;

        /// <summary>
        /// Recipe ingredients
        /// </summary>
        public List<RecipeIngredient> Ingredients { get; set; } = new List<RecipeIngredient>();

        /// <summary>
        /// Cooking steps
        /// </summary>
        public List<CookingStep> CookingSteps { get; set; } = new List<CookingStep>();

        /// <summary>
        /// Recipe nutrition information
        /// </summary>
        public RecipeNutrition? Nutrition { get; set; }
    }

    /// <summary>
    /// Recipe ingredient DTO
    /// </summary>
    public class RecipeIngredient : BaseEntityDTO
    {
        /// <summary>
        /// Recipe ID
        /// </summary>
        public int RecipeId { get; set; }

        /// <summary>
        /// Ingredient ID
        /// </summary>
        public int IngredientId { get; set; }

        /// <summary>
        /// Ingredient name
        /// </summary>
        public string IngredientName { get; set; } = string.Empty;

        /// <summary>
        /// Amount of ingredient
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Unit of measurement
        /// </summary>
        public string Unit { get; set; } = string.Empty;

        /// <summary>
        /// Is ingredient available in user pantry
        /// </summary>
        public bool IsAvailable { get; set; }
    }

    /// <summary>
    /// Cooking step DTO
    /// </summary>
    public class CookingStep : BaseEntityDTO
    {
        /// <summary>
        /// Recipe ID
        /// </summary>
        public int RecipeId { get; set; }

        /// <summary>
        /// Step number
        /// </summary>
        public int StepNumber { get; set; }

        /// <summary>
        /// Step instructions
        /// </summary>
        public string Instruction { get; set; } = string.Empty;

        /// <summary>
        /// Timing in minutes
        /// </summary>
        public int TimingMinutes { get; set; }
    }

    /// <summary>
    /// Recipe nutrition DTO
    /// </summary>
    public class RecipeNutrition : BaseEntityDTO
    {
        /// <summary>
        /// Recipe ID
        /// </summary>
        public int RecipeId { get; set; }

        /// <summary>
        /// Total calories
        /// </summary>
        public decimal TotalCalories { get; set; }

        /// <summary>
        /// Total protein in grams
        /// </summary>
        public decimal TotalProteinG { get; set; }

        /// <summary>
        /// Total carbohydrates in grams
        /// </summary>
        public decimal TotalCarbsG { get; set; }

        /// <summary>
        /// Total fat in grams
        /// </summary>
        public decimal TotalFatG { get; set; }

        /// <summary>
        /// Total fiber in grams
        /// </summary>
        public decimal TotalFiberG { get; set; }

        /// <summary>
        /// Total sugar in grams
        /// </summary>
        public decimal TotalSugarG { get; set; }

        /// <summary>
        /// Total sodium in grams
        /// </summary>
        public decimal TotalSodiumG { get; set; }

        /// <summary>
        /// Calories per serving
        /// </summary>
        public decimal CaloriesPerServing { get; set; }
    }

    /// <summary>
    /// Ingredient DTO
    /// </summary>
    public class Ingredient : BaseEntityDTO
    {
        /// <summary>
        /// Ingredient name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Ingredient category
        /// </summary>
        public string Category { get; set; } = string.Empty;
    }

    /// <summary>
    /// Detailed ingredient with nutrition
    /// </summary>
    public class IngredientDetail : Ingredient
    {
        /// <summary>
        /// Ingredient nutrition information
        /// </summary>
        public IngredientNutrition? Nutrition { get; set; }
    }

    /// <summary>
    /// Ingredient nutrition DTO
    /// </summary>
    public class IngredientNutrition : BaseEntityDTO
    {
        /// <summary>
        /// Ingredient ID
        /// </summary>
        public int IngredientId { get; set; }

        /// <summary>
        /// Calories per 100g
        /// </summary>
        public decimal CaloriesPer100g { get; set; }

        /// <summary>
        /// Protein in grams per 100g
        /// </summary>
        public decimal ProteinG { get; set; }

        /// <summary>
        /// Carbohydrates in grams per 100g
        /// </summary>
        public decimal CarbsG { get; set; }

        /// <summary>
        /// Fat in grams per 100g
        /// </summary>
        public decimal FatG { get; set; }

        /// <summary>
        /// Fiber in grams per 100g
        /// </summary>
        public decimal FiberG { get; set; }

        /// <summary>
        /// Sugar in grams per 100g
        /// </summary>
        public decimal SugarG { get; set; }

        /// <summary>
        /// Sodium in milligrams per 100g
        /// </summary>
        public decimal SodiumMg { get; set; }
    }

    /// <summary>
    /// Category DTO
    /// </summary>
    public class Category : BaseEntityDTO
    {
        /// <summary>
        /// Category name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Category description
        /// </summary>
        public string? Description { get; set; }
    }

    /// <summary>
    /// Meal plan DTO
    /// </summary>
    public class MealPlan : BaseEntityDTO
    {
        /// <summary>
        /// User ID who owns this meal plan
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Meal plan name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Start date
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// End date
        /// </summary>
        public DateTime EndDate { get; set; }
    }

    /// <summary>
    /// Detailed meal plan with items
    /// </summary>
    public class MealPlanDetail : MealPlan
    {
        /// <summary>
        /// Meal plan items
        /// </summary>
        public List<MealPlanItem> MealPlanItems { get; set; } = new List<MealPlanItem>();
    }

    /// <summary>
    /// Meal plan item DTO
    /// </summary>
    public class MealPlanItem : BaseEntityDTO
    {
        /// <summary>
        /// Meal plan ID
        /// </summary>
        public int MealPlanId { get; set; }

        /// <summary>
        /// Recipe ID
        /// </summary>
        public int RecipeId { get; set; }

        /// <summary>
        /// Recipe name
        /// </summary>
        public string RecipeName { get; set; } = string.Empty;

        /// <summary>
        /// Planned date
        /// </summary>
        public DateTime PlannedDate { get; set; }

        /// <summary>
        /// Meal type (breakfast, lunch, dinner, etc.)
        /// </summary>
        public string MealType { get; set; } = string.Empty;

        /// <summary>
        /// Number of servings
        /// </summary>
        public int Servings { get; set; }

        /// <summary>
        /// Recipe with nutrition information
        /// </summary>
        public RecipeWithNutrition? Recipe { get; set; }
    }

    /// <summary>
    /// Recipe with nutrition information
    /// </summary>
    public class RecipeWithNutrition
    {
        /// <summary>
        /// Recipe ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Recipe name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Default servings
        /// </summary>
        public int DefaultServings { get; set; }

        /// <summary>
        /// Recipe nutrition information
        /// </summary>
        public RecipeNutrition? Nutrition { get; set; }
    }

    /// <summary>
    /// Shopping list DTO
    /// </summary>
    public class ShoppingList : BaseEntityDTO
    {
        /// <summary>
        /// User ID who owns this shopping list
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Shopping list name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Created date
        /// </summary>
        public DateTime CreatedDate { get; set; }
        
        public List<ShoppingListItemDto> Items { get; set; } = new();
        
    }

    /// <summary>
    /// Detailed shopping list with items
    /// </summary>
    public class ShoppingListDetail : ShoppingList
    {
        /// <summary>
        /// Shopping list items
        /// </summary>
        public List<ShoppingListItem> Items { get; set; } = new List<ShoppingListItem>();

        /// <summary>
        /// Total number of items
        /// </summary>
        public int TotalItems => Items.Count;

        /// <summary>
        /// Number of purchased items
        /// </summary>
        public int PurchasedItems => Items.Count(i => i.Purchased);
    }

    /// <summary>
    /// Shopping list item DTO
    /// </summary>
    public class ShoppingListItem : BaseEntityDTO
    {
        /// <summary>
        /// Shopping list ID
        /// </summary>
        public int ShoppingListId { get; set; }

        /// <summary>
        /// Ingredient ID
        /// </summary>
        public int IngredientId { get; set; }

        /// <summary>
        /// Ingredient name
        /// </summary>
        public string IngredientName { get; set; } = string.Empty;

        /// <summary>
        /// Amount of ingredient
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Is item purchased
        /// </summary>
        public bool Purchased { get; set; }
    }

    /// <summary>
    /// User pantry item DTO
    /// </summary>
    public class UserPantryItem : BaseEntityDTO
    {
        /// <summary>
        /// User ID who owns this pantry item
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Ingredient ID
        /// </summary>
        public int IngredientId { get; set; }

        /// <summary>
        /// Ingredient name
        /// </summary>
        public string IngredientName { get; set; } = string.Empty;

        /// <summary>
        /// Amount in stock
        /// </summary>
        public decimal AmountInStock { get; set; }

        /// <summary>
        /// Unit of measurement
        /// </summary>
        public string Unit { get; set; } = string.Empty;

        /// <summary>
        /// Storage location
        /// </summary>
        public string? Location { get; set; }

        /// <summary>
        /// Expiry date
        /// </summary>
        public DateTime ExpiryDate { get; set; }

        /// <summary>
        /// Ingredient category
        /// </summary>
        public string? IngredientCategory { get; set; }

        /// <summary>
        /// Ingredient nutrition information
        /// </summary>
        public IngredientNutrition? Nutrition { get; set; }
    }

    /// <summary>
    /// Recipe match result
    /// </summary>
    public class RecipeMatch
    {
        /// <summary>
        /// Recipe
        /// </summary>
        public Recipe? Recipe { get; set; }

        /// <summary>
        /// Match percentage 0-100
        /// </summary>
        public int MatchPercentage { get; set; }

        /// <summary>
        /// Missing ingredients
        /// </summary>
        public List<Ingredient> MissingIngredients { get; set; } = new List<Ingredient>();
    }

    /// <summary>
    /// Recipe with ingredient availability
    /// </summary>
    public class RecipeWithAvailability
    {
        /// <summary>
        /// Recipe details
        /// </summary>
        public RecipeDetail? Recipe { get; set; }

        /// <summary>
        /// Recipe ingredients with availability flag
        /// </summary>
        public List<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();
    }
    
    /// <summary>
    /// View model for Recipe Finder feature
    /// </summary>
    public class RecipeFinderViewModel
    {
        /// <summary>
        /// Ingredients available in user's pantry
        /// </summary>
        public List<Ingredient> PantryIngredients { get; set; } = new List<Ingredient>();
    
        /// <summary>
        /// All available ingredients in the system
        /// </summary>
        public List<Ingredient> AllIngredients { get; set; } = new List<Ingredient>();
    
        /// <summary>
        /// IDs of selected ingredients for recipe matching
        /// </summary>
        public List<int> SelectedIngredientIds { get; set; } = new List<int>();
    }
    
    public class RecipeCategory
    {
        public int RecipeId { get; set; }
        public int CategoryId { get; set; }
    }
    
}