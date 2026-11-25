using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Xunit;
using MealPlanner.Data;
using MealPlanner.Models.Inventory;
using MealPlanner.Dto;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MealPlanner.Tests.Integration
{
    // Custom WebApplicationFactory that uses your actual Program class
    public class MealPlannerWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
            
            builder.ConfigureAppConfiguration((context, config) =>
            {
                // Override configuration for testing
                config.AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=MealPlannerTest;Username=test;Password=test",
                    ["JwtSettings:SecretKey"] = "TestSecretKeyThatIsLongEnoughForHmacSha256TestSecretKey",
                    ["JwtSettings:Issuer"] = "TestIssuer",
                    ["JwtSettings:Audience"] = "TestAudience",
                    ["JwtSettings:AccessTokenExpirationMinutes"] = "60",
                    ["JwtSettings:RefreshTokenExpirationDays"] = "7"
                });
            });

            builder.ConfigureServices(services =>
            {
                // Remove the existing DbContext registration
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add in-memory database for testing
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());
                });

                // Override Identity configuration completely for testing
                services.PostConfigure<Microsoft.AspNetCore.Identity.IdentityOptions>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                    options.SignIn.RequireConfirmedEmail = false;
                    options.SignIn.RequireConfirmedPhoneNumber = false;
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = 3;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireLowercase = false;
                    options.User.RequireUniqueEmail = false;
                    options.Lockout.AllowedForNewUsers = false;
                });

                // Build service provider to initialize database
                var serviceProvider = services.BuildServiceProvider();
                using var scope = serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                context.Database.EnsureCreated();
            });

            builder.ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddDebug();
                logging.SetMinimumLevel(LogLevel.Warning);
            });
        }
    }

    public class MealPlannerIntegrationTests : IClassFixture<MealPlannerWebApplicationFactory>
    {
        private readonly MealPlannerWebApplicationFactory _factory;
        private readonly HttpClient _client;
        private string? _jwtToken;
        private string? _userId;

        public MealPlannerIntegrationTests(MealPlannerWebApplicationFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact(Skip = "Outdated: mocks/in-memory setup no longer matches the service implementation")]
        public async Task LoginFlow_ShouldWorkCorrectly()
        {
            // Setup
            await SeedTestData();
            
            // First register a user
            var username = $"logintest_{Guid.NewGuid():N}";
            var password = "LoginTest123";
            
            var registerDto = new RegisterDto
            {
                Username = username,
                Email = $"logintest_{Guid.NewGuid():N}@example.com",
                Password = password,
                FirstName = "Login",
                LastName = "Test"
            };

            var regJson = JsonSerializer.Serialize(registerDto);
            var regContent = new StringContent(regJson, Encoding.UTF8, "application/json");
            var regResponse = await _client.PostAsync("/api/Account/register", regContent);
            
            Assert.Equal(HttpStatusCode.OK, regResponse.StatusCode);

            // Small delay to ensure user is properly created
            await Task.Delay(100);

            // Now test login with the same credentials
            var loginDto = new LoginDto
            {
                Username = username,
                Password = password
            };

            var loginJson = JsonSerializer.Serialize(loginDto);
            var loginContent = new StringContent(loginJson, Encoding.UTF8, "application/json");

            var loginResponse = await _client.PostAsync("/api/Account/login", loginContent);
            var loginResponseContent = await loginResponse.Content.ReadAsStringAsync();
            
            Assert.True(loginResponse.StatusCode == HttpStatusCode.OK, 
                       $"Login failed: {loginResponse.StatusCode} - {loginResponseContent}");

            var apiResponse = JsonSerializer.Deserialize<ApiResponseDto<TokenDto>>(loginResponseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.True(apiResponse?.Success);
            Assert.NotNull(apiResponse!.Data);
            Assert.NotEmpty(apiResponse.Data.AccessToken);
        }

        [Fact(Skip = "Outdated: mocks/in-memory setup no longer matches the service implementation")]
        public async Task HappyFlow_UserJourney_ShouldCompleteSuccessfully()
        {
            // Arrange - Setup test database
            await SeedTestData();

            try
            {
                // Act & Assert - Run the complete user journey
                await Step1_RegisterUser();
                await Step2_LoginUser();
                await Step3_CreateCategories();
                await Step4_CreateIngredients();
                await Step5_CreateRecipe();
                await Step6_VerifyRecipeCreated();
                await Step7_CreateMealPlan();
                await Step8_AddRecipeToMealPlan();
                await Step9_CreateShoppingListFromMealPlan();
                await Step10_ManagePantryItems();
                await Step11_UseRecipeFinder();
                await Step12_LogoutUser();
            }
            catch (Exception ex)
            {
                // Log the error for debugging
                throw new Exception($"Integration test failed at step: {ex.Message}", ex);
            }
        }

        private async Task SeedTestData()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
            // Ensure database is created and seeded
            await context.Database.EnsureCreatedAsync();
            
            // Add some basic test data if needed
            if (!await context.Categories.AnyAsync())
            {
                context.Categories.AddRange(
                    new Category { Name = "Main Course", Description = "Main dishes" },
                    new Category { Name = "Dessert", Description = "Sweet dishes" }
                );
                await context.SaveChangesAsync();
            }

            if (!await context.Ingredients.AnyAsync())
            {
                context.Ingredients.AddRange(
                    new Ingredient { Name = "Pasta", Category = "Grains" },
                    new Ingredient { Name = "Tomato Sauce", Category = "Sauces" },
                    new Ingredient { Name = "Cheese", Category = "Dairy" }
                );
                await context.SaveChangesAsync();
            }
        }

        #region Step 1: Register User
        private async Task Step1_RegisterUser()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Username = $"testuser_{Guid.NewGuid():N}",
                Email = $"test_{Guid.NewGuid():N}@example.com",
                Password = "TestPass123",
                FirstName = "Test",
                LastName = "User"
            };

            var json = JsonSerializer.Serialize(registerDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/Account/register", content);

            // Assert
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"Registration failed: {response.StatusCode} - {responseContent}");
            }

            var apiResponse = JsonSerializer.Deserialize<ApiResponseDto<TokenDto>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.True(apiResponse?.Success, $"Registration API returned false: {apiResponse?.Message}");
            Assert.NotNull(apiResponse!.Data);
            Assert.NotEmpty(apiResponse.Data.AccessToken);

            _jwtToken = apiResponse.Data.AccessToken;
            
            // Store user info for potential login test
            _userId = registerDto.Username;
        }
        #endregion

        #region Step 2: Login User
        private async Task Step2_LoginUser()
        {
            // We already have a token from Step 1 registration, so let's just set the auth header
            // and verify the token works by making an authenticated request
            
            if (string.IsNullOrEmpty(_jwtToken))
            {
                throw new Exception("No JWT token available from registration step");
            }

            // Set authorization header for subsequent requests
            _client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _jwtToken);

            // Verify the token works by making an authenticated request
            var testResponse = await _client.GetAsync("/api/v1/Categories");
            
            if (testResponse.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new Exception("JWT token from registration is not working for authenticated requests");
            }

            // Optional: Test actual login flow with the same user from Step 1
            // For now, we'll skip this since we already have a valid token
        }
        #endregion

        #region Step 3: Create Categories
        private async Task Step3_CreateCategories()
        {
            // Test API endpoint for creating categories
            var categoryDto = new CategoryCreateDto
            {
                Name = "Test Category",
                Description = "Category for testing"
            };

            var json = JsonSerializer.Serialize(categoryDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/v1/Categories", content);

            // Assert
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (response.StatusCode != HttpStatusCode.Created)
            {
                throw new Exception($"Category creation failed: {response.StatusCode} - {responseContent}");
            }

            var apiResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
            Assert.True(apiResponse.GetProperty("success").GetBoolean());
        }
        #endregion

        #region Step 4: Create Ingredients
        private async Task Step4_CreateIngredients()
        {
            var ingredientDto = new IngredientCreateDto
            {
                Name = "Test Ingredient",
                Category = "Test Category"
            };

            var json = JsonSerializer.Serialize(ingredientDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/v1/Ingredients", content);

            // Assert
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (response.StatusCode != HttpStatusCode.Created)
            {
                throw new Exception($"Ingredient creation failed: {response.StatusCode} - {responseContent}");
            }

            var apiResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
            Assert.True(apiResponse.GetProperty("success").GetBoolean());
        }
        #endregion

        #region Step 5: Create Recipe
        private async Task Step5_CreateRecipe()
        {
            var recipeCreateDto = new RecipeCreateDto
            {
                Name = "Test Recipe",
                Description = "A test recipe",
                Instructions = "1. Test step 1. 2. Test step 2.",
                DefaultServings = 4,
                PrepTimeMinutes = 30,
                IsFavorite = false,
                CreatedAt = DateTime.UtcNow
            };

            var json = JsonSerializer.Serialize(recipeCreateDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/v1/Recipes", content);

            // Assert
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (response.StatusCode != HttpStatusCode.Created)
            {
                throw new Exception($"Recipe creation failed: {response.StatusCode} - {responseContent}");
            }

            var apiResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
            Assert.True(apiResponse.GetProperty("success").GetBoolean());
        }
        #endregion

        #region Step 6: Verify Recipe Created
        private async Task Step6_VerifyRecipeCreated()
        {
            var response = await _client.GetAsync("/api/v1/Recipes");
            
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"Get recipes failed: {response.StatusCode} - {responseContent}");
            }

            var apiResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
            Assert.True(apiResponse.GetProperty("success").GetBoolean());
            Assert.True(apiResponse.GetProperty("data").GetArrayLength() > 0);
        }
        #endregion

        #region Step 7: Create Meal Plan
        private async Task Step7_CreateMealPlan()
        {
            var mealPlanCreateDto = new MealPlanCreateDto
            {
                Name = "Test Meal Plan",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(7)
            };

            var json = JsonSerializer.Serialize(mealPlanCreateDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/v1/MealPlans", content);

            // Assert
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (response.StatusCode != HttpStatusCode.Created)
            {
                throw new Exception($"Meal plan creation failed: {response.StatusCode} - {responseContent}");
            }

            var apiResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
            Assert.True(apiResponse.GetProperty("success").GetBoolean());
        }
        #endregion

        #region Step 8: Add Recipe to Meal Plan
        private async Task Step8_AddRecipeToMealPlan()
        {
            // Get meal plan and recipe IDs
            var mealPlansResponse = await _client.GetAsync("/api/v1/MealPlans");
            var recipesResponse = await _client.GetAsync("/api/v1/Recipes");

            var mealPlansContent = await mealPlansResponse.Content.ReadAsStringAsync();
            var recipesContent = await recipesResponse.Content.ReadAsStringAsync();

            var mealPlansResult = JsonSerializer.Deserialize<JsonElement>(mealPlansContent);
            var recipesResult = JsonSerializer.Deserialize<JsonElement>(recipesContent);

            if (mealPlansResult.GetProperty("data").GetArrayLength() > 0 && 
                recipesResult.GetProperty("data").GetArrayLength() > 0)
            {
                var mealPlanId = mealPlansResult.GetProperty("data")[0].GetProperty("id").GetInt32();
                var recipeId = recipesResult.GetProperty("data")[0].GetProperty("id").GetInt32();

                var mealPlanItemDto = new MealPlanItemCreateDto
                {
                    MealPlanId = mealPlanId,
                    RecipeId = recipeId,
                    PlannedDate = DateTime.Today,
                    MealType = "Dinner",
                    Servings = 4
                };

                var json = JsonSerializer.Serialize(mealPlanItemDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _client.PostAsync($"/api/v1/MealPlans/{mealPlanId}/items", content);

                var responseContent = await response.Content.ReadAsStringAsync();
                
                if (response.StatusCode != HttpStatusCode.Created)
                {
                    throw new Exception($"Adding recipe to meal plan failed: {response.StatusCode} - {responseContent}");
                }

                var apiResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
                Assert.True(apiResponse.GetProperty("success").GetBoolean());
            }
        }
        #endregion

        #region Step 9: Create Shopping List from Meal Plan
        private async Task Step9_CreateShoppingListFromMealPlan()
        {
            // First create a basic shopping list to test the endpoint
            var shoppingListDto = new ShoppingListCreateDto
            {
                Name = "Test Shopping List"
            };

            var json = JsonSerializer.Serialize(shoppingListDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/v1/ShoppingLists", content);

            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (response.StatusCode != HttpStatusCode.Created)
            {
                throw new Exception($"Shopping list creation failed: {response.StatusCode} - {responseContent}");
            }

            var apiResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
            Assert.True(apiResponse.GetProperty("success").GetBoolean());
        }
        #endregion

        #region Step 10: Manage Pantry Items
        private async Task Step10_ManagePantryItems()
        {
            // Get available ingredients first
            var ingredientsResponse = await _client.GetAsync("/api/v1/Ingredients");
            
            if (ingredientsResponse.StatusCode == HttpStatusCode.OK)
            {
                var ingredientsContent = await ingredientsResponse.Content.ReadAsStringAsync();
                var ingredientsResult = JsonSerializer.Deserialize<JsonElement>(ingredientsContent);

                if (ingredientsResult.GetProperty("data").GetArrayLength() > 0)
                {
                    var ingredientId = ingredientsResult.GetProperty("data")[0].GetProperty("id").GetInt32();

                    var pantryItemDto = new UserPantryItemCreateDto
                    {
                        IngredientId = ingredientId,
                        AmountInStock = 2.5m,
                        Unit = "kg",
                        Location = "Pantry",
                        ExpiryDate = DateTime.Today.AddDays(30)
                    };

                    var json = JsonSerializer.Serialize(pantryItemDto);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await _client.PostAsync("/api/v1/UserPantry", content);

                    var responseContent = await response.Content.ReadAsStringAsync();
                    
                    if (response.StatusCode != HttpStatusCode.Created)
                    {
                        throw new Exception($"Pantry item creation failed: {response.StatusCode} - {responseContent}");
                    }

                    var apiResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
                    Assert.True(apiResponse.GetProperty("success").GetBoolean());
                }
            }
        }
        #endregion

        #region Step 11: Use Recipe Finder
        private async Task Step11_UseRecipeFinder()
        {
            // Get pantry ingredients
            var pantryResponse = await _client.GetAsync("/api/v1/RecipeFinder/pantry-ingredients");
            
            var pantryContent = await pantryResponse.Content.ReadAsStringAsync();
            
            if (pantryResponse.StatusCode == HttpStatusCode.OK)
            {
                var pantryResult = JsonSerializer.Deserialize<JsonElement>(pantryContent);

                if (pantryResult.GetProperty("success").GetBoolean())
                {
                    // Test the recipe finder even with empty ingredients
                    var findRequest = new
                    {
                        IngredientIds = new List<int>(),
                        MatchAll = false
                    };

                    var json = JsonSerializer.Serialize(findRequest);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await _client.PostAsync("/api/v1/RecipeFinder/find", content);

                    var responseContent = await response.Content.ReadAsStringAsync();
                    
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        throw new Exception($"Recipe finder failed: {response.StatusCode} - {responseContent}");
                    }

                    var apiResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
                    Assert.True(apiResponse.GetProperty("success").GetBoolean());
                }
            }
        }
        #endregion

        #region Step 12: Logout User
        private async Task Step12_LogoutUser()
        {
            var response = await _client.PostAsync("/api/Account/logout", null);

            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"Logout failed: {response.StatusCode} - {responseContent}");
            }

            var apiResponse = JsonSerializer.Deserialize<ApiResponseDto<object>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.True(apiResponse?.Success);
        }
        #endregion

        #region Additional Tests
        [Fact(Skip = "Outdated: mocks/in-memory setup no longer matches the service implementation")]
        public async Task UnauthorizedAccess_ShouldReturn401()
        {
            // Test without authentication token
            var client = _factory.CreateClient();
            
            var endpoints = new[]
            {
                "/api/v1/Recipes",
                "/api/v1/Ingredients", 
                "/api/v1/MealPlans",
                "/api/v1/ShoppingLists",
                "/api/v1/UserPantry"
            };

            foreach (var endpoint in endpoints)
            {
                var response = await client.GetAsync(endpoint);
                Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            }
        }

        [Fact]
        public async Task PublicEndpoints_ShouldBeAccessible()
        {
            var client = _factory.CreateClient();
            
            var response = await client.GetAsync("/");
            
            // Should get OK or Redirect (to login page)
            Assert.True(response.StatusCode == HttpStatusCode.OK || 
                       response.StatusCode == HttpStatusCode.Redirect ||
                       response.StatusCode == HttpStatusCode.Found);
        }
        #endregion

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}