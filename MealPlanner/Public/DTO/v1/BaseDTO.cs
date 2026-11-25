// MealPlanner.Public.DTO/v1/BaseDTO.cs
using System;
using System.Collections.Generic;
using System.Text;

namespace MealPlanner.Public.DTO.v1
{
    /// <summary>
    /// Base DTO for all API v1 DTOs
    /// </summary>
    public abstract class BaseDTO
    {
        /// <summary>
        /// Entity ID
        /// </summary>
        public int Id { get; set; }
    }
    
    /// <summary>
    /// API Response DTO
    /// </summary>
    /// <typeparam name="T">Response data type</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Indicates if the request was successful
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        /// Response message
        /// </summary>
        public string Message { get; set; } = string.Empty;
        
        /// <summary>
        /// Response data
        /// </summary>
        public T? Data { get; set; }
    }
    
    /// <summary>
    /// Recipe DTO
    /// </summary>
    public class Recipe : BaseDTO
    {
        /// <summary>
        /// Recipe name
        /// </summary>
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// Recipe description
        /// </summary>
        public string? Description { get; set; }
        
        /// <summary>
        /// Number of servings
        /// </summary>
        public int DefaultServings { get; set; }
        
        /// <summary>
        /// Preparation time in minutes
        /// </summary>
        public int PrepTimeMinutes { get; set; }
        
        /// <summary>
        /// Is recipe marked as favorite
        /// </summary>
        public bool IsFavorite { get; set; }
        
        /// <summary>
        /// Date recipe was created
        /// </summary>
        public DateTime CreatedAt { get; set; }
        
        /// <summary>
        /// Recipe image URL
        /// </summary>
        public string? ImageUrl { get; set; }
        
        /// <summary>
        /// Recipe categories
        /// </summary>
        public List<Category> Categories { get; set; } = new List<Category>();
    }
    
    /// <summary>
    /// Detailed recipe with instructions, ingredients and steps
    /// </summary>
    public class RecipeDetail : Recipe
    {
        /// <summary>
        /// Recipe instructions
        /// </summary>
        public string? Instructions { get; set; }
        
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
    /// Recipe ingredients
    /// </summary>
    public class RecipeIngredient : BaseDTO
    {
        /// <summary>
        /// Ingredient ID
        /// </summary>
        public int IngredientId { get; set; }
        
        /// <summary>
        /// Ingredient name
        /// </summary>
        public string? IngredientName { get; set; }
        
        /// <summary>
        /// Amount of ingredient
        /// </summary>
        public decimal Amount { get; set; }
        
        /// <summary>
        /// Unit of measurement
        /// </summary>
        public string? Unit { get; set; }
    }
    
    /// <summary>
    /// Cooking step
    /// </summary>
    public class CookingStep : BaseDTO
    {
        /// <summary>
        /// Step number
        /// </summary>
        public int StepNumber { get; set; }
        
        /// <summary>
        /// Step instruction
        /// </summary>
        public string? Instruction { get; set; }
        
        /// <summary>
        /// Timing in minutes
        /// </summary>
        public int TimingMinutes { get; set; }
    }
    
    /// <summary>
    /// Recipe nutrition information
    /// </summary>
    public class RecipeNutrition : BaseDTO
    {
        /// <summary>
        /// Total calories
        /// </summary>
        public decimal TotalCalories { get; set; }
        
        /// <summary>
        /// Total protein in grams
        /// </summary>
        public decimal TotalProteinG { get; set; }
        
        /// <summary>
        /// Total carbs in grams
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
    /// Category DTO
    /// </summary>
    public class Category : BaseDTO
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
    /// Ingredient DTO
    /// </summary>
    public class Ingredient : BaseDTO
    {
        /// <summary>
        /// Ingredient name
        /// </summary>
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// Ingredient category
        /// </summary>
        public string? Category { get; set; }
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
    /// Ingredient nutrition information
    /// </summary>
    public class IngredientNutrition : BaseDTO
    {
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
    /// Meal plan DTO
    /// </summary>
    public class MealPlan : BaseDTO
    {
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
    public class MealPlanItem : BaseDTO
    {
        /// <summary>
        /// Recipe ID
        /// </summary>
        public int RecipeId { get; set; }
        
        /// <summary>
        /// Recipe name
        /// </summary>
        public string? RecipeName { get; set; }
        
        /// <summary>
        /// Planned date
        /// </summary>
        public DateTime PlannedDate { get; set; }
        
        /// <summary>
        /// Meal type (breakfast, lunch, dinner, etc.)
        /// </summary>
        public string? MealType { get; set; }
        
        /// <summary>
        /// Number of servings
        /// </summary>
        public int Servings { get; set; }
    }
    
    /// <summary>
    /// Shopping list DTO
    /// </summary>
    public class ShoppingList : BaseDTO
    {
        /// <summary>
        /// Shopping list name
        /// </summary>
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// Created date
        /// </summary>
        public DateTime CreatedDate { get; set; }
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
    public class ShoppingListItem : BaseDTO
    {
        /// <summary>
        /// Ingredient ID
        /// </summary>
        public int IngredientId { get; set; }
        
        /// <summary>
        /// Ingredient name
        /// </summary>
        public string? IngredientName { get; set; }
        
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
    public class UserPantryItem : BaseDTO
    {
        /// <summary>
        /// Ingredient ID
        /// </summary>
        public int IngredientId { get; set; }
        
        /// <summary>
        /// Ingredient name
        /// </summary>
        public string? IngredientName { get; set; }
        
        /// <summary>
        /// Amount in stock
        /// </summary>
        public decimal AmountInStock { get; set; }
        
        /// <summary>
        /// Unit of measurement
        /// </summary>
        public string? Unit { get; set; }
        
        /// <summary>
        /// Storage location
        /// </summary>
        public string? Location { get; set; }
        
        /// <summary>
        /// Expiry date
        /// </summary>
        public DateTime ExpiryDate { get; set; }
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
}