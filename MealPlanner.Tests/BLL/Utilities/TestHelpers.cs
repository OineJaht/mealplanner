// MealPlanner.Tests/Utilities/TestHelpers.cs
using System;
using System.Collections.Generic;
using AutoMapper;
using MealPlanner.MappingProfiles;
using MealPlanner.Models.Inventory;
using MealPlanner.BLL.DTO;
using Xunit;
// using Category = MealPlanner.Public.DTO.v1.Category;
// using CookingStep = MealPlanner.Public.DTO.v1.CookingStep;
// using IngredientNutrition = MealPlanner.Public.DTO.v1.IngredientNutrition;
// using MealPlan = MealPlanner.Public.DTO.v1.MealPlan;
// using MealPlanItem = MealPlanner.Public.DTO.v1.MealPlanItem;
// using RecipeIngredient = MealPlanner.Public.DTO.v1.RecipeIngredient;
// using ShoppingList = MealPlanner.Public.DTO.v1.ShoppingList;
// using ShoppingListItem = MealPlanner.Public.DTO.v1.ShoppingListItem;
// using UserPantryItem = MealPlanner.Public.DTO.v1.UserPantryItem;
// Entity aliases (for domain models)
using RecipeEntity = MealPlanner.Models.Inventory.Recipe;
using IngredientEntity = MealPlanner.Models.Inventory.Ingredient;
using CategoryEntity = MealPlanner.Models.Inventory.Category;
using CookingStepEntity = MealPlanner.Models.Inventory.CookingStep;
using RecipeIngredientEntity = MealPlanner.Models.Inventory.RecipeIngredient;
using IngredientNutritionEntity = MealPlanner.Models.Inventory.IngredientNutrition;
using MealPlanEntity = MealPlanner.Models.Inventory.MealPlan;
using MealPlanItemEntity = MealPlanner.Models.Inventory.MealPlanItem;
using ShoppingListEntity = MealPlanner.Models.Inventory.ShoppingList;
using ShoppingListItemEntity = MealPlanner.Models.Inventory.ShoppingListItem;
using UserPantryItemEntity = MealPlanner.Models.Inventory.UserPantryItem;
using RecipeCategoryEntity = MealPlanner.Models.Inventory.RecipeCategory;

// DTO aliases (for service layer)
using RecipeDto = MealPlanner.BLL.DTO.Recipe;
using CategoryDto = MealPlanner.BLL.DTO.Category;
using CookingStepDto = MealPlanner.BLL.DTO.CookingStep;
using IngredientNutritionDto = MealPlanner.BLL.DTO.IngredientNutrition;
using MealPlanDto = MealPlanner.BLL.DTO.MealPlan;
using MealPlanItemDto = MealPlanner.BLL.DTO.MealPlanItem;
using RecipeIngredientDto = MealPlanner.BLL.DTO.RecipeIngredient;
using ShoppingListDto = MealPlanner.BLL.DTO.ShoppingList;
using ShoppingListItemDto = MealPlanner.BLL.DTO.ShoppingListItem;
using UserPantryItemDto = MealPlanner.BLL.DTO.UserPantryItem;


namespace MealPlanner.Tests.Utilities
{
    /// <summary>
    /// Helper methods and utilities for testing
    /// </summary>
    public static class TestHelpers
    {
        /// <summary>
        /// Creates a configured AutoMapper for testing
        /// </summary>
        public static IMapper CreateMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfiles.MappingProfiles>();
            }, Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory.Instance);
            return config.CreateMapper();
        }

        /// <summary>
        /// Creates test user ID
        /// </summary>
        public static string CreateTestUserId() => "test-user-" + Guid.NewGuid().ToString("N")[..8];

        /// <summary>
        /// Creates a test recipe entity
        /// </summary>
        public static RecipeEntity CreateTestRecipe(int id = 1, string? userId = null, string? name = null)
        {
            return new RecipeEntity
            {
                Id = id,
                Name = name ?? $"Test Recipe {id}",
                UserId = userId ?? CreateTestUserId(),
                Description = $"Test description for recipe {id}",
                DefaultServings = 4,
                PrepTimeMinutes = 30,
                CreatedAt = DateTime.UtcNow,
                IsFavorite = false,
                Instructions = $"Test instructions for recipe {id}",
                RecipeCategories = new List<RecipeCategoryEntity>(),
                RecipeIngredients = new List<RecipeIngredientEntity>(),
                CookingSteps = new List<CookingStepEntity>()
            };
        }

        /// <summary>
        /// Creates a test recipe DTO
        /// </summary>
        public static RecipeDto CreateTestRecipeDto(int id = 1, string? userId = null, string? name = null)
        {
            return new RecipeDto
            {
                Id = id,
                Name = name ?? $"Test Recipe {id}",
                UserId = userId ?? CreateTestUserId(),
                Description = $"Test description for recipe {id}",
                DefaultServings = 4,
                PrepTimeMinutes = 30,
                CreatedAt = DateTime.UtcNow,
                IsFavorite = false,
                Categories = new List<CategoryDto>()
            };
        }

        /// <summary>
        /// Creates a test ingredient entity
        /// </summary>
        public static IngredientEntity CreateTestIngredient(int id = 1, string? name = null, string? category = null)
        {
            return new IngredientEntity
            {
                Id = id,
                Name = name ?? $"Test Ingredient {id}",
                Category = category ?? "Test Category",
                IngredientNutrition = CreateTestIngredientNutrition(id),
                RecipeIngredients = new List<RecipeIngredientEntity>(),
                UserPantryItems = new List<UserPantryItemEntity>(),
                ShoppingListItems = new List<ShoppingListItemEntity>()
            };
        }

        /// <summary>
        /// Creates test ingredient nutrition
        /// </summary>
        public static IngredientNutritionEntity CreateTestIngredientNutrition(int ingredientId = 1)
        {
            return new IngredientNutritionEntity
            {
                Id = ingredientId,
                IngredientId = ingredientId,
                CaloriesPer100g = 100 + ingredientId,
                ProteinG = 10 + ingredientId,
                CarbsG = 20 + ingredientId,
                FatG = 5 + ingredientId,
                FiberG = 3 + ingredientId,
                SugarG = 8 + ingredientId,
                SodiumMg = 50 + ingredientId
            };
        }

        /// <summary>
        /// Creates a test category entity
        /// </summary>
        public static CategoryEntity CreateTestCategory(int id = 1, string? name = null)
        {
            return new CategoryEntity
            {
                Id = id,
                Name = name ?? $"Test Category {id}",
                Description = $"Test description for category {id}",
                RecipeCategories = new List<RecipeCategoryEntity>()
            };
        }

        /// <summary>
        /// Creates a test cooking step
        /// </summary>
        public static CookingStepEntity CreateTestCookingStep(int id = 1, int recipeId = 1, int stepNumber = 1)
        {
            return new CookingStepEntity
            {
                Id = id,
                RecipeId = recipeId,
                StepNumber = stepNumber,
                Instruction = $"Test instruction {stepNumber}",
                TimingMinutes = 5 * stepNumber
            };
        }

        /// <summary>
        /// Creates a test recipe ingredient
        /// </summary>
        public static RecipeIngredientEntity CreateTestRecipeIngredient(int id = 1, int recipeId = 1, int ingredientId = 1)
        {
            return new RecipeIngredientEntity
            {
                Id = id,
                RecipeId = recipeId,
                IngredientId = ingredientId,
                Amount = 100 + id,
                Unit = "g"
            };
        }

        /// <summary>
        /// Creates a test meal plan
        /// </summary>
        public static MealPlanEntity CreateTestMealPlan(int id = 1, string? userId = null)
        {
            return new MealPlanEntity
            {
                Id = id,
                UserId = userId ?? CreateTestUserId(),
                Name = $"Test Meal Plan {id}",
                StartDate = DateTime.UtcNow.Date,
                EndDate = DateTime.UtcNow.Date.AddDays(7),
                MealPlanItems = new List<MealPlanItemEntity>()
            };
        }

        /// <summary>
        /// Creates a test shopping list
        /// </summary>
        public static ShoppingListEntity CreateTestShoppingList(int id = 1, string? userId = null)
        {
            return new ShoppingListEntity
            {
                Id = id,
                UserId = userId ?? CreateTestUserId(),
                Name = $"Test Shopping List {id}",
                CreatedDate = DateTime.UtcNow,
                Items = new List<ShoppingListItemEntity>()
            };
        }

        /// <summary>
        /// Creates a test user pantry item
        /// </summary>
        public static UserPantryItemEntity CreateTestUserPantryItem(int id = 1, string? userId = null, int ingredientId = 1)
        {
            return new UserPantryItemEntity
            {
                Id = id,
                UserId = userId ?? CreateTestUserId(),
                IngredientId = ingredientId,
                AmountInStock = 500,
                Unit = "g",
                Location = "Pantry",
                ExpiryDate = DateTime.UtcNow.AddDays(30)
            };
        }

        /// <summary>
        /// Verifies that two recipes are equivalent
        /// </summary>
        public static void AssertRecipesEqual(RecipeEntity expected, RecipeEntity actual)
        {
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Name, actual.Name);
            Assert.Equal(expected.Description, actual.Description);
            Assert.Equal(expected.UserId, actual.UserId);
            Assert.Equal(expected.DefaultServings, actual.DefaultServings);
            Assert.Equal(expected.PrepTimeMinutes, actual.PrepTimeMinutes);
            Assert.Equal(expected.IsFavorite, actual.IsFavorite);
        }

        /// <summary>
        /// Verifies that two recipe DTOs are equivalent
        /// </summary>
        public static void AssertRecipeDtosEqual(RecipeDto expected, RecipeDto actual)
        {
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Name, actual.Name);
            Assert.Equal(expected.Description, actual.Description);
            Assert.Equal(expected.UserId, actual.UserId);
            Assert.Equal(expected.DefaultServings, actual.DefaultServings);
            Assert.Equal(expected.PrepTimeMinutes, actual.PrepTimeMinutes);
            Assert.Equal(expected.IsFavorite, actual.IsFavorite);
        }

        /// <summary>
        /// Creates a list of test recipes
        /// </summary>
        public static List<RecipeEntity> CreateTestRecipes(int count = 3, string? userId = null)
        {
            var recipes = new List<RecipeEntity>();
            for (int i = 1; i <= count; i++)
            {
                recipes.Add(CreateTestRecipe(i, userId, $"Recipe {i}"));
            }
            return recipes;
        }

        /// <summary>
        /// Creates a list of test ingredients
        /// </summary>
        public static List<IngredientEntity> CreateTestIngredients(int count = 3)
        {
            var ingredients = new List<IngredientEntity>();
            for (int i = 1; i <= count; i++)
            {
                ingredients.Add(CreateTestIngredient(i, $"Ingredient {i}", $"Category {i % 2 + 1}"));
            }
            return ingredients;
        }
    }
}



