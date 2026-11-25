// MealPlanner.Tests/Utilities/TestDbContextFactory.cs
using Microsoft.EntityFrameworkCore;
using MealPlanner.Data;
using MealPlanner.Models.Inventory;
using CategoryEntity = MealPlanner.Models.Inventory.Category;
using IngredientEntity = MealPlanner.Models.Inventory.Ingredient;
using IngredientNutritionEntity = MealPlanner.Models.Inventory.IngredientNutrition;
using Microsoft.EntityFrameworkCore.InMemory; // Add this line

namespace MealPlanner.Tests.Utilities
{
    /// <summary>
    /// Factory for creating test database contexts
    /// </summary>
    public static class TestDbContextFactory
    {
        /// <summary>
        /// Creates an in-memory database context for testing
        /// </summary>
        public static ApplicationDbContext CreateInMemoryContext(string? databaseName = null)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        /// <summary>
        /// Seeds the database with test data
        /// </summary>
        public static async Task SeedTestDataAsync(ApplicationDbContext context)
        {
            // Add test categories - use entities, not DTOs
            var categories = new List<CategoryEntity>
            {
                new CategoryEntity { Id = 1, Name = "Main Course", Description = "Main dishes" },
                new CategoryEntity { Id = 2, Name = "Dessert", Description = "Sweet treats" },
                new CategoryEntity { Id = 3, Name = "Appetizer", Description = "Starters" }
            };
            context.Categories.AddRange(categories);

            // Add test ingredients - already using entities correctly
            var ingredients = new List<IngredientEntity>
            {
                new IngredientEntity { Id = 1, Name = "Chicken Breast", Category = "Meat" },
                new IngredientEntity { Id = 2, Name = "Rice", Category = "Grains" },
                new IngredientEntity { Id = 3, Name = "Broccoli", Category = "Vegetables" },
                new IngredientEntity { Id = 4, Name = "Olive Oil", Category = "Oils" }
            };
            context.Ingredients.AddRange(ingredients);

            // Add ingredient nutrition - use entities, not DTOs
            var nutrition = new List<IngredientNutritionEntity>
            {
                new IngredientNutritionEntity 
                { 
                    Id = 1, 
                    IngredientId = 1, 
                    CaloriesPer100g = 165, 
                    ProteinG = 31, 
                    CarbsG = 0, 
                    FatG = 3.6m 
                },
                new IngredientNutritionEntity 
                { 
                    Id = 2, 
                    IngredientId = 2, 
                    CaloriesPer100g = 130, 
                    ProteinG = 2.7m, 
                    CarbsG = 28, 
                    FatG = 0.3m 
                }
            };
            context.IngredientNutritions.AddRange(nutrition);

            await context.SaveChangesAsync();
        }
    }
}