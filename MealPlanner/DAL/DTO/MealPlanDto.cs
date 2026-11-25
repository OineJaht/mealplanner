// MealPlanner.DAL.DTO/MealPlanDto.cs
using System;
using System.Collections.Generic;

namespace MealPlanner.DAL.DTO
{
    /// <summary>
    /// Meal Plan DTO
    /// </summary>
    public class MealPlanDto : BaseUserOwnedDto
    {
        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ICollection<MealPlanItemDto> MealPlanItems { get; set; } = new List<MealPlanItemDto>();
    }

    /// <summary>
    /// Meal Plan Item DTO
    /// </summary>
    public class MealPlanItemDto : BaseDto
    {
        public int MealPlanId { get; set; }
        public int RecipeId { get; set; }
        public DateTime PlannedDate { get; set; }
        public string MealType { get; set; } = string.Empty; // breakfast, lunch, dinner, snack
        public int Servings { get; set; }
        public RecipeDto? Recipe { get; set; }
    }

    /// <summary>
    /// Shopping List DTO
    /// </summary>
    public class ShoppingListDto : BaseUserOwnedDto
    {
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public ICollection<ShoppingListItemDto> Items { get; set; } = new List<ShoppingListItemDto>();
    }

    /// <summary>
    /// Shopping List Item DTO
    /// </summary>
    public class ShoppingListItemDto : BaseDto
    {
        public int ShoppingListId { get; set; }
        public int IngredientId { get; set; }
        public decimal Amount { get; set; }
        public bool Purchased { get; set; }
        public IngredientDto? Ingredient { get; set; }
    }

    /// <summary>
    /// User Pantry Item DTO
    /// </summary>
    public class UserPantryItemDto : BaseUserOwnedDto
    {
        public int IngredientId { get; set; }
        public decimal AmountInStock { get; set; }
        public string Unit { get; set; } = string.Empty;
        public string? Location { get; set; }
        public DateTime ExpiryDate { get; set; }
        public IngredientDto? Ingredient { get; set; }
    }
}