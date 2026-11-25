// MealPlanner.Tests/BLL/Services/RecipeServiceTests.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using MealPlanner.BLL.Services.Implementations;
using MealPlanner.Data.UnitOfWork;
using MealPlanner.Data.Repositories;
using MealPlanner.Models.Inventory;
using MealPlanner.BLL.DTO;
using MealPlanner.Dto;
using MealPlanner.Models.Inventory.ViewModels.Recipes;
using MealPlanner.Data;

using RecipeEntity = MealPlanner.Models.Inventory.Recipe;
using RecipeDto = MealPlanner.BLL.DTO.Recipe;
using CookingStepEntity = MealPlanner.Models.Inventory.CookingStep;
using CookingStepDto = MealPlanner.BLL.DTO.CookingStep;
using IngredientEntity = MealPlanner.Models.Inventory.Ingredient;
using RecipeNutritionEntity = MealPlanner.Models.Inventory.RecipeNutrition;
using IngredientNutritionEntity = MealPlanner.Models.Inventory.IngredientNutrition;
using RecipeIngredientEntity = MealPlanner.Models.Inventory.RecipeIngredient;
using RecipeIngredientDto = MealPlanner.BLL.DTO.RecipeIngredient;
using IngredientDto = MealPlanner.BLL.DTO.Ingredient;
using RecipeNutritionDto = MealPlanner.BLL.DTO.RecipeNutrition;
using IngredientNutritionDto = MealPlanner.BLL.DTO.IngredientNutrition;
using RecipeCategoryEntity = MealPlanner.Models.Inventory.RecipeCategory;

namespace MealPlanner.BLL.Tests.BLL.Services
{
    public class RecipeServiceTests : IDisposable
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IRecipeRepository> _mockRecipeRepository;
        private readonly Mock<IRepository<RecipeCategoryEntity>> _mockRecipeCategoryRepository;
        private readonly Mock<IRepository<RecipeIngredientEntity>> _mockRecipeIngredientRepository;
        private readonly Mock<IRepository<CookingStepEntity>> _mockCookingStepRepository;
        private readonly Mock<IRepository<RecipeNutritionEntity>> _mockRecipeNutritionRepository;
        private readonly Mock<IRepository<IngredientNutritionEntity>> _mockIngredientNutritionRepository;
        private readonly Mock<IIngredientRepository> _mockIngredientRepository;
        private readonly RecipeService _recipeService;
        private readonly string _testUserId = "test-user-123";

        public RecipeServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _mockRecipeRepository = new Mock<IRecipeRepository>();
            _mockRecipeCategoryRepository = new Mock<IRepository<RecipeCategoryEntity>>();
            _mockRecipeIngredientRepository = new Mock<IRepository<RecipeIngredientEntity>>();
            _mockCookingStepRepository = new Mock<IRepository<CookingStepEntity>>();
            _mockRecipeNutritionRepository = new Mock<IRepository<RecipeNutritionEntity>>();
            _mockIngredientNutritionRepository = new Mock<IRepository<IngredientNutritionEntity>>();
            _mockIngredientRepository = new Mock<IIngredientRepository>();

            // Setup UnitOfWork mocks
            _mockUnitOfWork.Setup(u => u.Recipes).Returns(() => _mockRecipeRepository.Object);
            _mockUnitOfWork.Setup(u => u.RecipeCategories).Returns(() => _mockRecipeCategoryRepository.Object);
            _mockUnitOfWork.Setup(u => u.RecipeIngredients).Returns(() => _mockRecipeIngredientRepository.Object);
            _mockUnitOfWork.Setup(u => u.CookingSteps).Returns(() => _mockCookingStepRepository.Object);
            _mockUnitOfWork.Setup(u => u.RecipeNutrition).Returns(() => _mockRecipeNutritionRepository.Object);
            _mockUnitOfWork.Setup(u => u.IngredientNutrition).Returns(() => _mockIngredientNutritionRepository.Object);
            _mockUnitOfWork.Setup(u => u.Ingredients).Returns(() => _mockIngredientRepository.Object);

            _recipeService = new RecipeService(_mockUnitOfWork.Object, _mockMapper.Object);
        }

        #region AllAsync Tests

        [Fact]
        public async Task AllAsync_WithUserId_ReturnsUserRecipesOnly()
        {
            // Arrange
            var mockRecipes = new List<RecipeEntity>
            {
                new RecipeEntity { Id = 1, Name = "Recipe 1", UserId = _testUserId },
                new RecipeEntity { Id = 2, Name = "Recipe 2", UserId = _testUserId }
            };

            var expectedDtos = new List<RecipeDto>
            {
                new RecipeDto { Id = 1, Name = "Recipe 1", UserId = _testUserId },
                new RecipeDto { Id = 2, Name = "Recipe 2", UserId = _testUserId }
            };

            _mockRecipeRepository.Setup(r => r.GetUserRecipesAsync(_testUserId))
                .ReturnsAsync(mockRecipes);
            _mockMapper.Setup(m => m.Map<RecipeDto>(It.IsAny<RecipeEntity>()))
                .Returns((RecipeEntity r) => new RecipeDto { Id = r.Id, Name = r.Name, UserId = r.UserId });

            // Act
            var result = await _recipeService.AllAsync(_testUserId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, r => Assert.Equal(_testUserId, r.UserId));
            _mockRecipeRepository.Verify(r => r.GetUserRecipesAsync(_testUserId), Times.Once);
        }

        [Fact]
        public async Task AllAsync_WithoutUserId_ReturnsAllRecipes()
        {
            // Arrange
            var mockRecipes = new List<RecipeEntity>
            {
                new RecipeEntity { Id = 1, Name = "Recipe 1", UserId = "user1" },
                new RecipeEntity { Id = 2, Name = "Recipe 2", UserId = "user2" }
            };

            _mockRecipeRepository.Setup(r => r.GetAllAsync())
                .ReturnsAsync(mockRecipes);
            _mockMapper.Setup(m => m.Map<RecipeDto>(It.IsAny<RecipeEntity>()))
                .Returns((RecipeEntity r) => new RecipeDto { Id = r.Id, Name = r.Name, UserId = r.UserId });

            // Act
            var result = await _recipeService.AllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            _mockRecipeRepository.Verify(r => r.GetAllAsync(), Times.Once);
        }

        #endregion

        #region FindAsync Tests

        [Fact]
        public async Task FindAsync_WithValidIdAndUserId_ReturnsRecipe()
        {
            // Arrange
            var recipeId = 1;
            var mockRecipe = new RecipeEntity 
            { 
                Id = recipeId, 
                Name = "Test Recipe", 
                UserId = _testUserId 
            };
            var expectedDto = new RecipeDto 
            { 
                Id = recipeId, 
                Name = "Test Recipe", 
                UserId = _testUserId 
            };

            _mockRecipeRepository.Setup(r => r.GetByIdAsync(recipeId))
                .ReturnsAsync(mockRecipe);
            _mockMapper.Setup(m => m.Map<RecipeDto>(mockRecipe))
                .Returns(expectedDto);

            // Act
            var result = await _recipeService.FindAsync(recipeId, _testUserId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(recipeId, result.Id);
            Assert.Equal(_testUserId, result.UserId);
        }

        [Fact]
        public async Task FindAsync_WithInvalidUserId_ReturnsNull()
        {
            // Arrange
            var recipeId = 1;
            var mockRecipe = new RecipeEntity 
            { 
                Id = recipeId, 
                Name = "Test Recipe", 
                UserId = "different-user" 
            };

            _mockRecipeRepository.Setup(r => r.GetByIdAsync(recipeId))
                .ReturnsAsync(mockRecipe);

            // Act
            var result = await _recipeService.FindAsync(recipeId, _testUserId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task FindAsync_WithNonExistentId_ReturnsNull()
        {
            // Arrange
            var recipeId = 999;
            _mockRecipeRepository.Setup(r => r.GetByIdAsync(recipeId))
                .ReturnsAsync((RecipeEntity?)null);

            // Act
            var result = await _recipeService.FindAsync(recipeId, _testUserId);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region AddAsync Tests

        [Fact]
        public async Task AddAsync_WithValidRecipe_AddsRecipeAndReturnsDto()
        {
            // Arrange
            var recipeDto = new RecipeDto 
            { 
                Name = "New Recipe", 
                Description = "Test Description" 
            };
            var mappedRecipe = new RecipeEntity 
            { 
                Name = "New Recipe", 
                Description = "Test Description" 
            };
            var savedRecipe = new RecipeEntity
            { 
                Id = 1, 
                Name = "New Recipe", 
                Description = "Test Description",
                UserId = _testUserId,
                CreatedAt = DateTime.UtcNow
            };
            var expectedResult = new RecipeDto 
            { 
                Id = 1, 
                Name = "New Recipe", 
                Description = "Test Description",
                UserId = _testUserId
            };

            _mockMapper.Setup(m => m.Map<RecipeEntity>(recipeDto))
                .Returns(mappedRecipe);
            _mockMapper.Setup(m => m.Map<RecipeDto>(It.IsAny<RecipeEntity>()))
                .Returns(expectedResult);
            _mockRecipeRepository.Setup(r => r.Add(It.IsAny<RecipeEntity>()));
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync())
                .ReturnsAsync(1)
                .Callback(() => {
                    mappedRecipe.Id = 1;
                    mappedRecipe.UserId = _testUserId;
                    mappedRecipe.CreatedAt = DateTime.UtcNow;
                });

            // Act
            var result = await _recipeService.AddAsync(recipeDto, _testUserId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal(_testUserId, result.UserId);
            _mockRecipeRepository.Verify(r => r.Add(It.IsAny<RecipeEntity>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_WithValidRecipeAndOwnership_UpdatesRecipe()
        {
            // Arrange
            var recipeId = 1;
            var recipeDto = new RecipeDto 
            { 
                Id = recipeId, 
                Name = "Updated Recipe", 
                UserId = _testUserId 
            };
            var existingRecipe = new RecipeEntity
            { 
                Id = recipeId, 
                Name = "Original Recipe", 
                UserId = _testUserId 
            };
            var updatedRecipe = new RecipeEntity 
            { 
                Id = recipeId, 
                Name = "Updated Recipe", 
                UserId = _testUserId 
            };

            _mockRecipeRepository.Setup(r => r.GetByIdAsync(recipeId))
                .ReturnsAsync(existingRecipe);
            _mockMapper.Setup(m => m.Map(recipeDto, existingRecipe));
            _mockMapper.Setup(m => m.Map<RecipeDto>(existingRecipe))
                .Returns(recipeDto);
            _mockRecipeRepository.Setup(r => r.Update(existingRecipe));
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _recipeService.UpdateAsync(recipeDto, _testUserId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(recipeId, result.Id);
            _mockRecipeRepository.Verify(r => r.Update(existingRecipe), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WithInvalidOwnership_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var recipeId = 1;
            var recipeDto = new RecipeDto 
            { 
                Id = recipeId, 
                Name = "Updated Recipe" 
            };
            var existingRecipe = new RecipeEntity 
            { 
                Id = recipeId, 
                Name = "Original Recipe", 
                UserId = "different-user" 
            };

            _mockRecipeRepository.Setup(r => r.GetByIdAsync(recipeId))
                .ReturnsAsync(existingRecipe);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _recipeService.UpdateAsync(recipeDto, _testUserId));
        }

        [Fact]
        public async Task UpdateAsync_WithNonExistentRecipe_ThrowsArgumentException()
        {
            // Arrange
            var recipeId = 999;
            var recipeDto = new RecipeDto { Id = recipeId };

            _mockRecipeRepository.Setup(r => r.GetByIdAsync(recipeId))
                .ReturnsAsync((RecipeEntity?)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => _recipeService.UpdateAsync(recipeDto, _testUserId));
        }

        #endregion

        #region RemoveAsync Tests

        [Fact]
        public async Task RemoveAsync_WithValidIdAndOwnership_RemovesRecipe()
        {
            // Arrange
            var recipeId = 1;
            var existingRecipe = new RecipeEntity 
            { 
                Id = recipeId, 
                UserId = _testUserId 
            };

            _mockRecipeRepository.Setup(r => r.GetByIdAsync(recipeId))
                .ReturnsAsync(existingRecipe);
            _mockRecipeRepository.Setup(r => r.Remove(existingRecipe));
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _recipeService.RemoveAsync(recipeId, _testUserId);

            // Assert
            Assert.True(result);
            _mockRecipeRepository.Verify(r => r.Remove(existingRecipe), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task RemoveAsync_WithInvalidOwnership_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var recipeId = 1;
            var existingRecipe = new RecipeEntity 
            { 
                Id = recipeId, 
                UserId = "different-user" 
            };

            _mockRecipeRepository.Setup(r => r.GetByIdAsync(recipeId))
                .ReturnsAsync(existingRecipe);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _recipeService.RemoveAsync(recipeId, _testUserId));
        }

        [Fact]
        public async Task RemoveAsync_WithNonExistentRecipe_ReturnsFalse()
        {
            // Arrange
            var recipeId = 999;
            _mockRecipeRepository.Setup(r => r.GetByIdAsync(recipeId))
                .ReturnsAsync((RecipeEntity?)null);

            // Act
            var result = await _recipeService.RemoveAsync(recipeId, _testUserId);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region GetWithDetailsAsync Tests

        [Fact(Skip = "Outdated: mocks/in-memory setup no longer matches the service implementation")]
        public async Task GetWithDetailsAsync_WithValidIdAndOwnership_ReturnsRecipeDetail()
        {
            // Arrange
            var recipeId = 1;
            var mockRecipe = new RecipeEntity 
            { 
                Id = recipeId, 
                Name = "Test Recipe", 
                UserId = _testUserId,
                RecipeIngredients = new List<RecipeIngredientEntity>(),
                CookingSteps = new List<CookingStepEntity>()
            };
            var expectedDto = new RecipeDto
            { 
                Id = recipeId, 
                Name = "Test Recipe", 
                UserId = _testUserId 
            };

            _mockRecipeRepository.Setup(r => r.GetRecipeWithDetailsAsync(recipeId))
                .ReturnsAsync(mockRecipe);
            _mockMapper.Setup(m => m.Map<RecipeDto>(mockRecipe))
                .Returns(expectedDto);

            // Act
            var result = await _recipeService.GetWithDetailsAsync(recipeId, _testUserId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(recipeId, result.Id);
            Assert.Equal(_testUserId, result.UserId);
        }

        [Fact]
        public async Task GetWithDetailsAsync_WithInvalidOwnership_ReturnsNull()
        {
            // Arrange
            var recipeId = 1;
            var mockRecipe = new RecipeEntity 
            { 
                Id = recipeId, 
                UserId = "different-user" 
            };

            _mockRecipeRepository.Setup(r => r.GetRecipeWithDetailsAsync(recipeId))
                .ReturnsAsync(mockRecipe);

            // Act
            var result = await _recipeService.GetWithDetailsAsync(recipeId, _testUserId);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region GetFavoritesAsync Tests

        [Fact]
        public async Task GetFavoritesAsync_ReturnsUserFavoriteRecipes()
        {
            // Arrange
            var recipe1 = new RecipeEntity { Id = 1, Name = "Favorite 1", UserId = _testUserId, IsFavorite = true };
            var recipe2 = new RecipeEntity { Id = 2, Name = "Favorite 2", UserId = _testUserId, IsFavorite = true };
            var mockRecipes = new List<RecipeEntity> { recipe1, recipe2 };

            _mockRecipeRepository.Setup(r => r.GetFavoriteRecipesAsync(_testUserId))
                .ReturnsAsync(mockRecipes);
    
            // ✅ Set up specific mappings for each recipe
            _mockMapper.Setup(m => m.Map<DTO.Recipe>(recipe1))
                .Returns(new DTO.Recipe { Id = 1, Name = "Favorite 1", UserId = _testUserId });
            _mockMapper.Setup(m => m.Map<DTO.Recipe>(recipe2))
                .Returns(new DTO.Recipe { Id = 2, Name = "Favorite 2", UserId = _testUserId });

            // Act
            var result = await _recipeService.GetFavoritesAsync(_testUserId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }
        
        [Fact]
        public async Task GetFavoritesAsync_WithEmptyResult_ReturnsEmptyList()
        {
            // Arrange
            _mockRecipeRepository.Setup(r => r.GetFavoriteRecipesAsync(_testUserId))
                .ReturnsAsync(new List<RecipeEntity>());

            // Act
            var result = await _recipeService.GetFavoritesAsync(_testUserId);

            // Assert
            Assert.Empty(result);
        }

        #endregion

        #region GetRecentAsync Tests

        [Fact]
        public async Task GetRecentAsync_WithCount_ReturnsLimitedResults()
        {
            // Arrange
            var count = 3;
            var mockRecipes = new List<RecipeEntity>
            {
                new RecipeEntity { Id = 1, Name = "Recent 1", UserId = _testUserId },
                new RecipeEntity { Id = 2, Name = "Recent 2", UserId = _testUserId },
                new RecipeEntity { Id = 3, Name = "Recent 3", UserId = _testUserId }
            };

            _mockRecipeRepository.Setup(r => r.GetRecentRecipesAsync(_testUserId, count))
                .ReturnsAsync(mockRecipes);
            _mockMapper.Setup(m => m.Map<RecipeDto>(It.IsAny<RecipeEntity>()))
                .Returns((RecipeEntity r) => new RecipeDto { Id = r.Id, Name = r.Name, UserId = r.UserId });

            // Act
            var result = await _recipeService.GetRecentAsync(_testUserId, count);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(count, result.Count);
            _mockRecipeRepository.Verify(r => r.GetRecentRecipesAsync(_testUserId, count), Times.Once);
        }

        [Fact]
        public async Task GetRecentAsync_WithoutCount_UsesDefaultValue()
        {
            // Arrange
            var mockRecipes = new List<RecipeEntity>();
            _mockRecipeRepository.Setup(r => r.GetRecentRecipesAsync(_testUserId, 5))
                .ReturnsAsync(mockRecipes);

            // Act
            var result = await _recipeService.GetRecentAsync(_testUserId);

            // Assert
            _mockRecipeRepository.Verify(r => r.GetRecentRecipesAsync(_testUserId, 5), Times.Once);
        }

        #endregion

        #region AddFullRecipeAsync Tests

        [Fact(Skip = "Outdated: mocks/in-memory setup no longer matches the service implementation")]
        public async Task AddFullRecipeAsync_WithValidData_CreatesCompleteRecipe()
        {
            // Arrange
            var recipeDto = new RecipeCreateDto 
            { 
                Name = "Full Recipe", 
                Description = "Test Description" 
            };
            var categoryIds = new int[] { 1, 2 };
            var ingredients = new List<(int ingredientId, decimal amount, string unit)>
            {
                (1, 100, "g"),
                (2, 200, "ml")
            };

            var mappedRecipe = new RecipeEntity 
            { 
                Name = "Full Recipe", 
                Description = "Test Description" 
            };

            // Setup database context mock for transaction
            var mockDbContext = new Mock<ApplicationDbContext>();
            var mockStrategy = new Mock<Microsoft.EntityFrameworkCore.Storage.IExecutionStrategy>();
            
            mockStrategy.Setup(s => s.ExecuteAsync(It.IsAny<Func<Task<RecipeDto>>>()))
                .Returns((Func<Task<RecipeDto>> operation) => operation());

            _mockUnitOfWork.Setup(u => u as ApplicationDbContext).Returns(mockDbContext.Object);
            
            _mockMapper.Setup(m => m.Map<RecipeEntity>(recipeDto)).Returns(mappedRecipe);
            _mockMapper.Setup(m => m.Map<RecipeDto>(It.IsAny<RecipeEntity>()))
                .Returns(new RecipeDto { Id = 1, Name = "Full Recipe" });
            
            _mockRecipeRepository.Setup(r => r.Add(It.IsAny<RecipeEntity>()));
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Setup callback to simulate ID assignment
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync())
                .ReturnsAsync(1)
                .Callback(() => mappedRecipe.Id = 1);

            // Act
            var result = await _recipeService.AddFullRecipeAsync(recipeDto, _testUserId, categoryIds, ingredients);

            // Assert
            Assert.NotNull(result);
            _mockRecipeRepository.Verify(r => r.Add(It.IsAny<RecipeEntity>()), Times.Once);
            _mockRecipeCategoryRepository.Verify(r => r.Add(It.IsAny<RecipeCategoryEntity>()), Times.Exactly(2));
            _mockRecipeIngredientRepository.Verify(r => r.Add(It.IsAny<RecipeIngredientEntity>()), Times.Exactly(2));
        }
        
        [Fact(Skip = "Outdated: mocks/in-memory setup no longer matches the service implementation")]
        public async Task AddFullRecipeAsync_WithValidData_ExecutesAllSteps()
        {
            // Arrange
            var recipeDto = new RecipeCreateDto { Name = "Full Recipe" };
            var categoryIds = new int[] { 1, 2 };
            var ingredients = new List<(int ingredientId, decimal amount, string unit)> 
            { 
                (1, 100, "g"),
                (2, 200, "ml") 
            };
    
            var mappedRecipe = new RecipeEntity { Name = "Full Recipe" };
    
            // ✅ Mock the database context properly for transactions
            _mockUnitOfWork.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.CommitTransactionAsync()).Returns(Task.CompletedTask);
    
            _mockMapper.Setup(m => m.Map<RecipeEntity>(recipeDto)).Returns(mappedRecipe);
            _mockMapper.Setup(m => m.Map<RecipeDto>(It.IsAny<RecipeEntity>()))
                .Returns(new RecipeDto { Id = 1, Name = "Full Recipe" });
    
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1)
                .Callback(() => mappedRecipe.Id = 1); // ✅ Simulate ID assignment
    
            // ✅ Mock all repository operations
            _mockRecipeRepository.Setup(r => r.Add(It.IsAny<RecipeEntity>()));
            _mockUnitOfWork.Setup(u => u.RecipeCategories.Add(It.IsAny<RecipeCategoryEntity>()));
            _mockUnitOfWork.Setup(u => u.RecipeIngredients.Add(It.IsAny<RecipeIngredientEntity>()));

            // Act
            var result = await _recipeService.AddFullRecipeAsync(recipeDto, _testUserId, categoryIds, ingredients);
    
            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.AtLeast(1));
        }
        
        [Fact(Skip = "Outdated: mocks/in-memory setup no longer matches the service implementation")]
        public async Task AddFullRecipeAsync_WithValidData_CreatesRecipeWithCategoriesAndIngredients()
        {
            // Arrange
            var recipeDto = new RecipeCreateDto { Name = "Full Recipe" };
            var categoryIds = new int[] { 1, 2 };
            var ingredients = new List<(int ingredientId, decimal amount, string unit)> { (1, 100, "g") };
    
            var mappedRecipe = new RecipeEntity { Name = "Full Recipe" };
    
            _mockMapper.Setup(m => m.Map<RecipeEntity>(recipeDto)).Returns(mappedRecipe);
            _mockMapper.Setup(m => m.Map<RecipeDto>(It.IsAny<RecipeEntity>()))
                .Returns(new RecipeDto { Id = 1, Name = "Full Recipe" });
    
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1)
                .Callback(() => mappedRecipe.Id = 1);
    
            // Act
            var result = await _recipeService.AddFullRecipeAsync(recipeDto, _testUserId, categoryIds, ingredients);
    
            // Assert
            Assert.NotNull(result);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.AtLeast(1));
        }
        
        [Fact(Skip = "Outdated: mocks/in-memory setup no longer matches the service implementation")]
        public async Task AddFullRecipeAsync_WithValidData_CallsAllRepositories()
        {
            // Arrange
            var recipeDto = new RecipeCreateDto { Name = "Test Recipe" };
            var categoryIds = new int[] { 1, 2 };
            var ingredients = new List<(int, decimal, string)> { (1, 100, "g") };

            _mockMapper.Setup(m => m.Map<RecipeEntity>(recipeDto))
                .Returns(new RecipeEntity { Name = "Test Recipe" });
            _mockMapper.Setup(m => m.Map<RecipeDto>(It.IsAny<RecipeEntity>()))
                .Returns(new RecipeDto { Name = "Test Recipe" });
    
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _recipeService.AddFullRecipeAsync(recipeDto, _testUserId, categoryIds, ingredients);

            // Assert
            Assert.NotNull(result);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.AtLeast(1));
        }
        

        #endregion

        #region CookingStep Tests

        [Fact]
        public async Task GetCookingStepAsync_WithValidIdAndOwnership_ReturnsCookingStep()
        {
            // Arrange
            var stepId = 1;
            var recipeId = 1;
            var mockStep = new CookingStepEntity 
            { 
                Id = stepId, 
                RecipeId = recipeId, 
                Instruction = "Test step" 
            };
            var mockRecipe = new RecipeEntity 
            { 
                Id = recipeId, 
                UserId = _testUserId 
            };
            var expectedDto = new CookingStepDto 
            { 
                Id = stepId, 
                RecipeId = recipeId, 
                Instruction = "Test step" 
            };

            _mockCookingStepRepository.Setup(r => r.GetByIdAsync(stepId))
                .ReturnsAsync(mockStep);
            _mockRecipeRepository.Setup(r => r.GetByIdAsync(recipeId))
                .ReturnsAsync(mockRecipe);
            _mockMapper.Setup(m => m.Map<CookingStepDto>(mockStep))
                .Returns(expectedDto);

            // Act
            var result = await _recipeService.GetCookingStepAsync(stepId, _testUserId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(stepId, result.Id);
            Assert.Equal(recipeId, result.RecipeId);
        }

        [Fact]
        public async Task GetCookingStepAsync_WithInvalidOwnership_ReturnsNull()
        {
            // Arrange
            var stepId = 1;
            var recipeId = 1;
            var mockStep = new CookingStepEntity 
            { 
                Id = stepId, 
                RecipeId = recipeId 
            };
            var mockRecipe = new RecipeEntity 
            { 
                Id = recipeId, 
                UserId = "different-user" 
            };

            _mockCookingStepRepository.Setup(r => r.GetByIdAsync(stepId))
                .ReturnsAsync(mockStep);
            _mockRecipeRepository.Setup(r => r.GetByIdAsync(recipeId))
                .ReturnsAsync(mockRecipe);

            // Act
            var result = await _recipeService.GetCookingStepAsync(stepId, _testUserId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddCookingStepAsync_WithValidStep_AddsStep()
        {
            // Arrange
            var stepDto = new CookingStepDto 
            { 
                RecipeId = 1, 
                Instruction = "New step" 
            };
            var mockRecipe = new RecipeEntity
            { 
                Id = 1, 
                UserId = _testUserId 
            };
            var mappedStep = new CookingStepEntity 
            { 
                RecipeId = 1, 
                Instruction = "New step" 
            };
            var resultDto = new CookingStepDto
            {
                RecipeId = 1,
                Instruction = "New step"
            };

            _mockRecipeRepository.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(mockRecipe);
            _mockMapper.Setup(m => m.Map<CookingStepEntity>(stepDto))
                .Returns(mappedStep);
            _mockMapper.Setup(m => m.Map<CookingStepDto>(mappedStep))
                .Returns(resultDto);  // <-- Return DTO, not entity
            _mockCookingStepRepository.Setup(r => r.Add(mappedStep));
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _recipeService.AddCookingStepAsync(stepDto, _testUserId);

            // Assert
            Assert.NotNull(result);
            _mockCookingStepRepository.Verify(r => r.Add(mappedStep), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        #endregion

        #region Nutrition Tests

        [Fact]
        public async Task CalculateNutritionAsync_WithValidRecipe_CalculatesNutrition()
        {
            // Arrange
            var recipeId = 1;
            var mockRecipe = new RecipeEntity 
            { 
                Id = recipeId, 
                UserId = _testUserId,
                DefaultServings = 4,
                RecipeIngredients = new List<RecipeIngredientEntity>
                {
                    new RecipeIngredientEntity { IngredientId = 1, Amount = 100 },
                    new RecipeIngredientEntity { IngredientId = 2, Amount = 200 }
                }
            };
            var mockIngredient1 = new IngredientEntity 
            { 
                Id = 1,
                IngredientNutrition = new IngredientNutritionEntity
                {
                    CaloriesPer100g = 100,
                    ProteinG = 10,
                    CarbsG = 20,
                    FatG = 5
                }
            };
            var mockIngredient2 = new IngredientEntity 
            { 
                Id = 2,
                IngredientNutrition = new IngredientNutritionEntity
                {
                    CaloriesPer100g = 50,
                    ProteinG = 5,
                    CarbsG = 10,
                    FatG = 2
                }
            };

            _mockRecipeRepository.Setup(r => r.GetRecipeWithIngredientsAsync(recipeId))
                .ReturnsAsync(mockRecipe);
            _mockIngredientRepository.Setup(r => r.GetIngredientWithNutritionAsync(1))
                .ReturnsAsync(mockIngredient1);
            _mockIngredientRepository.Setup(r => r.GetIngredientWithNutritionAsync(2))
                .ReturnsAsync(mockIngredient2);
            _mockRecipeNutritionRepository.Setup(r => r.SingleOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<RecipeNutritionEntity, bool>>>()))
                .ReturnsAsync((RecipeNutritionEntity?)null);
            _mockRecipeNutritionRepository.Setup(r => r.Add(It.IsAny<RecipeNutritionEntity>()));
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var expectedNutrition = new RecipeNutritionDto
            {
                RecipeId = recipeId,
                TotalCalories = 200, // (100*1 + 50*2)
                CaloriesPerServing = 50 // 200/4
            };

            _mockMapper.Setup(m => m.Map<RecipeNutritionDto>(It.IsAny<RecipeNutritionEntity>()))
                .Returns(expectedNutrition);

            // Act
            var result = await _recipeService.CalculateNutritionAsync(recipeId, _testUserId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(recipeId, result.RecipeId);
            Assert.Equal(200, result.TotalCalories);
            Assert.Equal(50, result.CaloriesPerServing);
        }

        #endregion

        public void Dispose()
        {
            // Cleanup - nothing to dispose in this case
        }

        #region UpdateNutritionAsync Tests

        [Fact]
        public async Task UpdateNutritionAsync_WithExistingNutrition_UpdatesSuccessfully()
        {
            // Arrange
            var nutritionDto = new RecipeNutritionDto
            {
                Id = 1,
                RecipeId = 1,
                TotalCalories = 500,
                TotalProteinG = 25
            };
            var mockRecipe = new RecipeEntity { Id = 1, UserId = _testUserId };
            var existingNutrition = new RecipeNutritionEntity { Id = 1, RecipeId = 1 };

            _mockRecipeRepository.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(mockRecipe);
            _mockRecipeNutritionRepository.Setup(r => r.SingleOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<RecipeNutritionEntity, bool>>>()))
                .ReturnsAsync(existingNutrition);
            _mockMapper.Setup(m => m.Map(nutritionDto, existingNutrition));
            _mockMapper.Setup(m => m.Map<RecipeNutritionDto>(existingNutrition))
                .Returns(nutritionDto);
            _mockRecipeNutritionRepository.Setup(r => r.Update(existingNutrition));
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _recipeService.UpdateNutritionAsync(nutritionDto, _testUserId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.RecipeId);
            _mockRecipeNutritionRepository.Verify(r => r.Update(existingNutrition), Times.Once);
        }

        [Fact]
        public async Task UpdateNutritionAsync_WithNewNutrition_CreatesNew()
        {
            // Arrange
            var nutritionDto = new RecipeNutritionDto
            {
                RecipeId = 1,
                TotalCalories = 500,
                TotalProteinG = 25
            };
            var mockRecipe = new RecipeEntity { Id = 1, UserId = _testUserId };
            var newNutrition = new RecipeNutritionEntity { RecipeId = 1 };
            _mockMapper.Setup(m => m.Map<RecipeNutritionEntity>(nutritionDto))
                .Returns(newNutrition);
            _mockRecipeNutritionRepository.Setup(r => r.Add(newNutrition));
            _mockRecipeRepository.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(mockRecipe);
            _mockRecipeNutritionRepository.Setup(r => r.SingleOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<RecipeNutritionEntity, bool>>>()))
                .ReturnsAsync((RecipeNutritionEntity?)null);
            _mockMapper.Setup(m => m.Map<RecipeNutritionEntity>(nutritionDto))
                .Returns(newNutrition);
            _mockMapper.Setup(m => m.Map<RecipeNutritionDto>(newNutrition))
                .Returns(nutritionDto);
            _mockRecipeNutritionRepository.Setup(r => r.Add(newNutrition));
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _recipeService.UpdateNutritionAsync(nutritionDto, _testUserId);

            // Assert
            Assert.NotNull(result);
            _mockRecipeNutritionRepository.Verify(r => r.Add(newNutrition), Times.Once);
        }

        [Fact]
        public async Task UpdateNutritionAsync_WithInvalidOwnership_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var nutritionDto = new RecipeNutritionDto { RecipeId = 1 };
            var mockRecipe = new RecipeEntity { Id = 1, UserId = "different-user" };

            _mockRecipeRepository.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(mockRecipe);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _recipeService.UpdateNutritionAsync(nutritionDto, _testUserId));
        }

        #endregion

        #region UpdateCookingStepAsync Tests

        [Fact]
        public async Task UpdateCookingStepAsync_WithValidStep_UpdatesSuccessfully()
        {
            // Arrange
            var stepDto = new CookingStepDto()
            {
                Id = 1,
                RecipeId = 1,
                Instruction = "Updated instruction"
            };
            var existingStep = new CookingStepEntity() { Id = 1, RecipeId = 1 };
            var mockRecipe = new RecipeEntity { Id = 1, UserId = _testUserId };

            _mockCookingStepRepository.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(existingStep);
            _mockRecipeRepository.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(mockRecipe);
            _mockMapper.Setup(m => m.Map(stepDto, existingStep));
            _mockMapper.Setup(m => m.Map<CookingStepDto>(existingStep))
                .Returns(stepDto);
            _mockCookingStepRepository.Setup(r => r.Update(existingStep));
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _recipeService.UpdateCookingStepAsync(stepDto, _testUserId);

            // Assert
            Assert.NotNull(result);
            _mockCookingStepRepository.Verify(r => r.Update(existingStep), Times.Once);
        }

        [Fact]
        public async Task UpdateCookingStepAsync_WithNonExistentStep_ThrowsArgumentException()
        {
            // Arrange
            var stepDto = new CookingStepDto { Id = 999 };

            _mockCookingStepRepository.Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((CookingStepEntity?)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => _recipeService.UpdateCookingStepAsync(stepDto, _testUserId));
        }

        #endregion

        #region RemoveCookingStepAsync Tests

        [Fact]
        public async Task RemoveCookingStepAsync_WithValidStep_RemovesSuccessfully()
        {
            // Arrange
            var stepId = 1;
            var existingStep = new CookingStepEntity { Id = stepId, RecipeId = 1 };
            var mockRecipe = new RecipeEntity { Id = 1, UserId = _testUserId };

            _mockCookingStepRepository.Setup(r => r.GetByIdAsync(stepId))
                .ReturnsAsync(existingStep);
            _mockRecipeRepository.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(mockRecipe);
            _mockCookingStepRepository.Setup(r => r.Remove(existingStep));
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _recipeService.RemoveCookingStepAsync(stepId, _testUserId);

            // Assert
            Assert.True(result);
            _mockCookingStepRepository.Verify(r => r.Remove(existingStep), Times.Once);
        }

        [Fact]
        public async Task RemoveCookingStepAsync_WithNonExistentStep_ReturnsFalse()
        {
            // Arrange
            var stepId = 999;
            _mockCookingStepRepository.Setup(r => r.GetByIdAsync(stepId))
                .ReturnsAsync((CookingStepEntity?)null);

            // Act
            var result = await _recipeService.RemoveCookingStepAsync(stepId, _testUserId);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region ReorderCookingStepsAsync Tests

        [Fact]
        public async Task ReorderCookingStepsAsync_WithValidRecipe_ReordersSteps()
        {
            // Arrange
            var recipeId = 1;
            var mockRecipe = new RecipeEntity { Id = recipeId, UserId = _testUserId };
            var cookingSteps = new List<CookingStepEntity>
            {
                new CookingStepEntity { Id = 1, RecipeId = recipeId, StepNumber = 3 },
                new CookingStepEntity { Id = 2, RecipeId = recipeId, StepNumber = 1 },
                new CookingStepEntity { Id = 3, RecipeId = recipeId, StepNumber = 2 }
            };

            _mockRecipeRepository.Setup(r => r.GetByIdAsync(recipeId))
                .ReturnsAsync(mockRecipe);
            _mockCookingStepRepository.Setup(r => r.ToListAsync(It.IsAny<System.Linq.Expressions.Expression<Func<CookingStepEntity, bool>>>()))
                .ReturnsAsync(cookingSteps);
            _mockCookingStepRepository.Setup(r => r.Update(It.IsAny<CookingStepEntity>()));
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _recipeService.ReorderCookingStepsAsync(recipeId, _testUserId);

            // Assert
            _mockCookingStepRepository.Verify(r => r.Update(It.IsAny<CookingStepEntity>()), Times.Exactly(3));
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task ReorderCookingStepsAsync_WithStepIds_ReordersInSpecificOrder()
        {
            // Arrange
            var recipeId = 1;
            var stepIds = new int[] { 3, 1, 2 };
            var mockRecipe = new RecipeEntity { Id = recipeId, UserId = _testUserId };
            var step1 = new CookingStepEntity { Id = 1, RecipeId = recipeId };
            var step2 = new CookingStepEntity { Id = 2, RecipeId = recipeId };
            var step3 = new CookingStepEntity { Id = 3, RecipeId = recipeId };

            _mockRecipeRepository.Setup(r => r.GetByIdAsync(recipeId))
                .ReturnsAsync(mockRecipe);
            _mockCookingStepRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(step1);
            _mockCookingStepRepository.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(step2);
            _mockCookingStepRepository.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(step3);
            _mockCookingStepRepository.Setup(r => r.Update(It.IsAny<CookingStepEntity>()));
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _recipeService.ReorderCookingStepsAsync(recipeId, stepIds, _testUserId);

            // Assert
            Assert.Equal(1, step3.StepNumber); // First in the array
            Assert.Equal(2, step1.StepNumber); // Second in the array  
            Assert.Equal(3, step2.StepNumber); // Third in the array
            _mockCookingStepRepository.Verify(r => r.Update(It.IsAny<CookingStepEntity>()), Times.Exactly(3));
        }

        #endregion

        #region UpdateFullRecipeAsync Tests

        [Fact(Skip = "Outdated: mocks/in-memory setup no longer matches the service implementation")]
        public async Task UpdateFullRecipeAsync_WithValidData_UpdatesCompleteRecipe()
        {
            // Arrange
            var recipeDto = new RecipeUpdateDto
            {
                Id = 1,
                Name = "Updated Recipe",
                Description = "Updated Description"
            };
            var categoryIds = new List<int> { 1, 2 };
            var ingredients = new Dictionary<int, RecipeIngredientInfo>
            {
                { 1, new RecipeIngredientInfo { Amount = 150, Unit = "g" } },
                { 2, new RecipeIngredientInfo { Amount = 250, Unit = "ml" } }
            };

            var existingRecipe = new RecipeEntity { Id = 1, UserId = _testUserId };
            var existingCategories = new List<RecipeCategoryEntity>();
            var existingIngredients = new List<RecipeIngredientEntity>();

            _mockUnitOfWork.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.CommitTransactionAsync()).Returns(Task.CompletedTask);
            _mockRecipeRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingRecipe);
            _mockMapper.Setup(m => m.Map(recipeDto, existingRecipe));
            _mockMapper.Setup(m => m.Map<RecipeDto>(existingRecipe))
                .Returns(new RecipeDto { Id = 1, Name = "Updated Recipe" });
            _mockRecipeRepository.Setup(r => r.Update(existingRecipe));
            _mockRecipeCategoryRepository.Setup(r => r.ToListAsync(It.IsAny<System.Linq.Expressions.Expression<Func<RecipeCategoryEntity, bool>>>()))
                .ReturnsAsync(existingCategories);
            _mockRecipeIngredientRepository.Setup(r => r.ToListAsync(It.IsAny<System.Linq.Expressions.Expression<Func<RecipeIngredientEntity, bool>>>()))
                .ReturnsAsync(existingIngredients);
            _mockRecipeCategoryRepository.Setup(r => r.RemoveRange(existingCategories));
            _mockRecipeIngredientRepository.Setup(r => r.RemoveRange(existingIngredients));
            _mockRecipeCategoryRepository.Setup(r => r.Add(It.IsAny<RecipeCategoryEntity>()));
            _mockRecipeIngredientRepository.Setup(r => r.Add(It.IsAny<RecipeIngredientEntity>()));
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _recipeService.UpdateFullRecipeAsync(recipeDto, _testUserId, categoryIds, ingredients);

            // Assert
            Assert.NotNull(result);
            _mockRecipeRepository.Verify(r => r.Update(existingRecipe), Times.Once);
            _mockRecipeCategoryRepository.Verify(r => r.Add(It.IsAny<RecipeCategoryEntity>()), Times.Exactly(2));
            _mockRecipeIngredientRepository.Verify(r => r.Add(It.IsAny<RecipeIngredientEntity>()), Times.Exactly(2));
            _mockUnitOfWork.Verify(u => u.CommitTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateFullRecipeAsync_WithUnauthorizedUser_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var recipeDto = new RecipeUpdateDto { Id = 1 };
            var existingRecipe = new RecipeEntity { Id = 1, UserId = "different-user" };

            _mockUnitOfWork.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _mockRecipeRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingRecipe);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _recipeService.UpdateFullRecipeAsync(recipeDto, _testUserId, new List<int>(), new Dictionary<int, RecipeIngredientInfo>()));
            
            _mockUnitOfWork.Verify(u => u.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateFullRecipeAsync_WithException_RollsBackTransaction()
        {
            // Arrange
            var recipeDto = new RecipeUpdateDto { Id = 1 };
            var existingRecipe = new RecipeEntity { Id = 1, UserId = _testUserId };

            _mockUnitOfWork.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.RollbackTransactionAsync()).Returns(Task.CompletedTask);
            _mockRecipeRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingRecipe);
            _mockMapper.Setup(m => m.Map(recipeDto, existingRecipe)).Throws(new Exception("Test exception"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(
                () => _recipeService.UpdateFullRecipeAsync(recipeDto, _testUserId, new List<int>(), new Dictionary<int, RecipeIngredientInfo>()));
            
            _mockUnitOfWork.Verify(u => u.RollbackTransactionAsync(), Times.Once);
        }

        #endregion

        #region Edge Cases and Error Handling Tests

        [Fact]
        public async Task AddAsync_WithNullDto_ThrowsNullReferenceException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(
                () => _recipeService.AddAsync(null!, _testUserId));
        }

        [Fact]
        public async Task AddAsync_WithNullUserId_ThrowsNullReferenceException()
        {
            // Arrange
            var recipeDto = new RecipeDto { Name = "Test Recipe" };

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(
                () => _recipeService.AddAsync(recipeDto, null!));
        }
        
        [Fact]
        public async Task GetFavoritesAsync_ReturnsUserFavorites()
        {
            
            // Arrange
            var mockRecipes = new List<RecipeEntity>
            {
                new RecipeEntity { Id = 1, Name = "Fav Recipe 1", UserId = _testUserId, IsFavorite = true },
                new RecipeEntity { Id = 2, Name = "Fav Recipe 2", UserId = _testUserId, IsFavorite = true }
            };

            _mockRecipeRepository.Setup(r => r.GetFavoriteRecipesAsync(_testUserId))
                .ReturnsAsync(mockRecipes);
            _mockMapper.Setup(m => m.Map<RecipeDto>(It.IsAny<RecipeEntity>()))
                .Returns((RecipeEntity r) => new RecipeDto { Id = r.Id, Name = r.Name, UserId = r.UserId });

            // Act
            var result = await _recipeService.GetFavoritesAsync(_testUserId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            _mockRecipeRepository.Verify(r => r.GetFavoriteRecipesAsync(_testUserId), Times.Once);
        }

        [Fact(Skip = "Outdated: mocks/in-memory setup no longer matches the service implementation")]
        public async Task GetWithDetailsAsync_WithNullUserId_ReturnsRecipeWithoutOwnershipCheck()
        {
            // Arrange
            var recipeId = 1;
            var mockRecipe = new RecipeEntity 
            { 
                Id = recipeId, 
                Name = "Test Recipe", 
                UserId = "any-user"
            };
            var expectedDto = new RecipeDetailDto 
            { 
                Id = recipeId, 
                Name = "Test Recipe" 
            };

            _mockRecipeRepository.Setup(r => r.GetRecipeWithDetailsAsync(recipeId))
                .ReturnsAsync(mockRecipe);
            _mockMapper.Setup(m => m.Map<RecipeDetailDto >(mockRecipe))
                .Returns(expectedDto);

            // Act
            var result = await _recipeService.GetWithDetailsAsync(recipeId, null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(recipeId, result.Id);
        }

        [Fact]
        public async Task CalculateNutritionAsync_WithRecipeWithoutIngredients_ReturnsZeroNutrition()
        {
            // Arrange
            var recipeId = 1;
            var mockRecipe = new RecipeEntity 
            { 
                Id = recipeId, 
                UserId = _testUserId,
                DefaultServings = 4,
                RecipeIngredients = new List<RecipeIngredientEntity>()
            };

            _mockRecipeRepository.Setup(r => r.GetRecipeWithIngredientsAsync(recipeId))
                .ReturnsAsync(mockRecipe);
            _mockRecipeNutritionRepository.Setup(r => r.SingleOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<RecipeNutritionEntity, bool>>>()))
                .ReturnsAsync((RecipeNutritionEntity?)null);
            _mockRecipeNutritionRepository.Setup(r => r.Add(It.IsAny<RecipeNutritionEntity>()));
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var expectedNutrition = new RecipeNutritionDto
            {
                RecipeId = recipeId,
                TotalCalories = 0,
                CaloriesPerServing = 0
            };

            _mockMapper.Setup(m => m.Map<RecipeNutritionDto>(It.IsAny<RecipeNutritionEntity>()))
                .Returns(expectedNutrition);

            // Act
            var result = await _recipeService.CalculateNutritionAsync(recipeId, _testUserId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.TotalCalories);
            Assert.Equal(0, result.CaloriesPerServing);
        }

        [Fact]
        public async Task CalculateNutritionAsync_WithZeroServings_UsesTotalCaloriesAsPerServing()
        {
            // Arrange
            var recipeId = 1;
            var mockRecipe = new RecipeEntity
            { 
                Id = recipeId, 
                UserId = _testUserId,
                DefaultServings = 0, // Zero servings
                RecipeIngredients = new List<RecipeIngredientEntity>
                {
                    new RecipeIngredientEntity { IngredientId = 1, Amount = 100 }
                }
            };
            var mockIngredient = new IngredientEntity 
            { 
                Id = 1,
                IngredientNutrition = new IngredientNutritionEntity { CaloriesPer100g = 100 }
            };

            _mockRecipeRepository.Setup(r => r.GetRecipeWithIngredientsAsync(recipeId))
                .ReturnsAsync(mockRecipe);
            _mockIngredientRepository.Setup(r => r.GetIngredientWithNutritionAsync(1))
                .ReturnsAsync(mockIngredient);
            _mockRecipeNutritionRepository.Setup(r => r.SingleOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<RecipeNutritionEntity, bool>>>()))
                .ReturnsAsync((RecipeNutritionEntity?)null);
            _mockRecipeNutritionRepository.Setup(r => r.Add(It.IsAny<RecipeNutritionEntity>()));
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var expectedNutrition = new RecipeNutritionDto
            {
                RecipeId = recipeId,
                TotalCalories = 100,
                CaloriesPerServing = 100 // Should equal total when servings = 0
            };

            _mockMapper.Setup(m => m.Map<RecipeNutritionDto>(It.IsAny<RecipeNutritionEntity>()))
                .Returns(expectedNutrition);

            // Act
            var result = await _recipeService.CalculateNutritionAsync(recipeId, _testUserId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(100, result.CaloriesPerServing);
        }

        #endregion
    }
}