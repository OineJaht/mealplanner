// MealPlanner/ApiControllers/v1/RecipesController.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MealPlanner.BLL.Services;
using MealPlanner.Security;

namespace MealPlanner.ApiControllers.v1
{
    /// <summary>
    /// API controller for recipe operations
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    public class RecipesController : ControllerBase
    {
        private readonly IAppBLL _bll;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bll">Business logic layer</param>
        /// <param name="mapper">Mapper</param>
        public RecipesController(IAppBLL bll, IMapper mapper)
        {
            _bll = bll;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all recipes for currently logged in user
        /// </summary>
        /// <returns>List of recipes</returns>
        [HttpGet]
        [ProducesResponseType(typeof(Public.DTO.v1.ApiResponse<IEnumerable<Public.DTO.v1.Recipe>>), 200)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<Public.DTO.v1.ApiResponse<IEnumerable<Public.DTO.v1.Recipe>>>> GetRecipes()
        {
            var userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new Public.DTO.v1.ApiResponse<IEnumerable<Public.DTO.v1.Recipe>>
                {
                    Success = false,
                    Message = "User not authenticated"
                });
            }

            var data = await _bll.Recipes.AllAsync(userId);
            var recipes = _mapper.Map<IEnumerable<Public.DTO.v1.Recipe>>(data);

            return Ok(new Public.DTO.v1.ApiResponse<IEnumerable<Public.DTO.v1.Recipe>>
            {
                Success = true,
                Data = recipes
            });
        }

        /// <summary>
        /// Get recipe by ID
        /// </summary>
        /// <param name="id">Recipe ID</param>
        /// <returns>Recipe details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Public.DTO.v1.ApiResponse<Public.DTO.v1.RecipeDetail>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<Public.DTO.v1.ApiResponse<Public.DTO.v1.RecipeDetail>>> GetRecipe(int id)
        {
            var userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new Public.DTO.v1.ApiResponse<Public.DTO.v1.RecipeDetail>
                {
                    Success = false,
                    Message = "User not authenticated"
                });
            }

            // Use GetWithDetailsAsync instead of regular FindAsync
            var recipe = await _bll.Recipes.GetWithDetailsAsync(id, userId);
            if (recipe == null)
            {
                return NotFound(new Public.DTO.v1.ApiResponse<Public.DTO.v1.RecipeDetail>
                {
                    Success = false,
                    Message = $"Recipe with ID {id} not found"
                });
            }
            
            // DEBUG: Check what GetWithDetailsAsync returns
            Console.WriteLine($"DEBUG GetRecipe: Recipe found - ID: {recipe.Id}");
            Console.WriteLine($"DEBUG GetRecipe: Instructions: '{recipe.Instructions}'");
            Console.WriteLine($"DEBUG GetRecipe: Ingredients count: {recipe.Ingredients?.Count ?? 0}");
            Console.WriteLine($"DEBUG GetRecipe: Categories count: {recipe.Categories?.Count ?? 0}");

            // Map to RecipeDetail, not Recipe
            var recipeDetail = _mapper.Map<Public.DTO.v1.RecipeDetail>(recipe);
            
            // DEBUG: Check what AutoMapper produces
            Console.WriteLine($"DEBUG GetRecipe: Mapped Instructions: '{recipeDetail.Instructions}'");
            Console.WriteLine($"DEBUG GetRecipe: Mapped Ingredients count: {recipeDetail.Ingredients?.Count ?? 0}");
            Console.WriteLine($"DEBUG GetRecipe: Mapped Categories count: {recipeDetail.Categories?.Count ?? 0}");

            return Ok(new Public.DTO.v1.ApiResponse<Public.DTO.v1.RecipeDetail>
            {
                Success = true,
                Data = recipeDetail
            });
        }

        /// <summary>
        /// Get favorite recipes for currently logged in user
        /// </summary>
        /// <returns>List of favorite recipes</returns>
        [HttpGet("favorites")]
        [ProducesResponseType(typeof(Public.DTO.v1.ApiResponse<IEnumerable<Public.DTO.v1.Recipe>>), 200)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<Public.DTO.v1.ApiResponse<IEnumerable<Public.DTO.v1.Recipe>>>> GetFavoriteRecipes()
        {
            var userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new Public.DTO.v1.ApiResponse<IEnumerable<Public.DTO.v1.Recipe>>
                {
                    Success = false,
                    Message = "User not authenticated"
                });
            }

            var favorites = await _bll.Recipes.GetFavoritesAsync(userId);
            var recipes = _mapper.Map<IEnumerable<Public.DTO.v1.Recipe>>(favorites);

            return Ok(new Public.DTO.v1.ApiResponse<IEnumerable<Public.DTO.v1.Recipe>>
            {
                Success = true,
                Data = recipes
            });
        }

        /// <summary>
        /// Get recent recipes for currently logged in user
        /// </summary>
        /// <param name="count">Number of recipes to return (default 5)</param>
        /// <returns>List of recent recipes</returns>
        [HttpGet("recent")]
        [ProducesResponseType(typeof(Public.DTO.v1.ApiResponse<IEnumerable<Public.DTO.v1.Recipe>>), 200)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<Public.DTO.v1.ApiResponse<IEnumerable<Public.DTO.v1.Recipe>>>> GetRecentRecipes([FromQuery] int count = 5)
        {
            var userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new Public.DTO.v1.ApiResponse<IEnumerable<Public.DTO.v1.Recipe>>
                {
                    Success = false,
                    Message = "User not authenticated"
                });
            }

            var recent = await _bll.Recipes.GetRecentAsync(userId, count);
            var recipes = _mapper.Map<IEnumerable<Public.DTO.v1.Recipe>>(recent);

            return Ok(new Public.DTO.v1.ApiResponse<IEnumerable<Public.DTO.v1.Recipe>>
            {
                Success = true,
                Data = recipes
            });
        }

        /// <summary>
/// Create a new recipe
/// </summary>
/// <param name="recipe">Recipe to create</param>
/// <returns>Created recipe</returns>
[HttpPost]
[ProducesResponseType(typeof(Public.DTO.v1.ApiResponse<Public.DTO.v1.Recipe>), 201)]
[ProducesResponseType(400)]
[ProducesResponseType(401)]
public async Task<ActionResult<Public.DTO.v1.ApiResponse<Public.DTO.v1.Recipe>>> CreateRecipe([FromBody] MealPlanner.Dto.RecipeCreateDto recipe)
{
    if (!ModelState.IsValid)
    {
        return BadRequest(new Public.DTO.v1.ApiResponse<Public.DTO.v1.Recipe>
        {
            Success = false,
            Message = "Invalid recipe data"
        });
    }

    var userId = User.GetUserId();
    // DEBUG: Add logging to see what's happening
    Console.WriteLine($"DEBUG: User.Identity.Name = {User.Identity?.Name}");
    Console.WriteLine($"DEBUG: UserId from GetUserId() = {userId}");
    Console.WriteLine($"DEBUG: User is authenticated = {User.Identity?.IsAuthenticated}");
    
    if (string.IsNullOrEmpty(userId))
    {
        Console.WriteLine("DEBUG: UserId is null or empty!");
        return Unauthorized(new Public.DTO.v1.ApiResponse<Public.DTO.v1.Recipe>
        {
            Success = false,
            Message = "User not authenticated"
        });
    }

    try
    {
        // Extract categories and ingredients from the DTO
        var categoryIds = recipe.CategoryIds?.ToArray() ?? new int[0];
        var ingredients = recipe.Ingredients?.Select(i => 
            (i.IngredientId, i.Amount, i.Unit ?? string.Empty)).ToList() 
            ?? new List<(int, decimal, string)>();

        Console.WriteLine($"DEBUG: About to call AddFullRecipeAsync with userId = {userId}");
        Console.WriteLine($"DEBUG: Recipe has {categoryIds.Length} categories and {ingredients.Count} ingredients");

        // Use the FULL recipe creation method that handles all related entities
        var createdRecipe = await _bll.Recipes.AddFullRecipeAsync(
            recipe, 
            userId, 
            categoryIds, 
            ingredients);
        
        var fullRecipe = await _bll.Recipes.GetWithDetailsAsync(createdRecipe.Id, userId);
        var result = _mapper.Map<Public.DTO.v1.RecipeDetail>(fullRecipe);

        return CreatedAtAction(nameof(GetRecipe), new { id = result.Id }, new Public.DTO.v1.ApiResponse<Public.DTO.v1.Recipe>
        {
            Success = true,
            Message = "Recipe created successfully",
            Data = result
        });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"DEBUG: Exception in CreateRecipe: {ex.Message}");
        Console.WriteLine($"DEBUG: Inner exception: {ex.InnerException?.Message}");
        Console.WriteLine($"DEBUG: Stack trace: {ex.StackTrace}");
        return BadRequest(new Public.DTO.v1.ApiResponse<Public.DTO.v1.Recipe>
        {
            Success = false,
            Message = $"Error creating recipe: {ex.Message}"
        });
    }
}

        /// <summary>
        /// Update an existing recipe
        /// </summary>
        /// <param name="id">Recipe ID</param>
        /// <param name="recipe">Updated recipe data</param>
        /// <returns>Updated recipe</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Public.DTO.v1.ApiResponse<Public.DTO.v1.Recipe>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<ActionResult<Public.DTO.v1.ApiResponse<Public.DTO.v1.Recipe>>> UpdateRecipe(int id, [FromBody] MealPlanner.Dto.RecipeUpdateDto recipe)
        {
            if (id != recipe.Id)
            {
                return BadRequest(new Public.DTO.v1.ApiResponse<Public.DTO.v1.Recipe>
                {
                    Success = false,
                    Message = "ID mismatch"
                });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new Public.DTO.v1.ApiResponse<Public.DTO.v1.Recipe>
                {
                    Success = false,
                    Message = "Invalid recipe data"
                });
            }

            var userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new Public.DTO.v1.ApiResponse<Public.DTO.v1.Recipe>
                {
                    Success = false,
                    Message = "User not authenticated"
                });
            }

            try
            {
                var existingRecipe = await _bll.Recipes.FindAsync(id, userId);
                if (existingRecipe == null)
                {
                    return NotFound(new Public.DTO.v1.ApiResponse<Public.DTO.v1.Recipe>
                    {
                        Success = false,
                        Message = $"Recipe with ID {id} not found"
                    });
                }

                var bllRecipe = _mapper.Map<BLL.DTO.Recipe>(recipe);
                var updatedRecipe = await _bll.Recipes.UpdateAsync(bllRecipe, userId);
                var result = _mapper.Map<Public.DTO.v1.Recipe>(updatedRecipe);

                return Ok(new Public.DTO.v1.ApiResponse<Public.DTO.v1.Recipe>
                {
                    Success = true,
                    Message = "Recipe updated successfully",
                    Data = result
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return BadRequest(new Public.DTO.v1.ApiResponse<Public.DTO.v1.Recipe>
                {
                    Success = false,
                    Message = $"Error updating recipe: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Delete a recipe
        /// </summary>
        /// <param name="id">Recipe ID</param>
        /// <returns>Success response</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(Public.DTO.v1.ApiResponse<object>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<ActionResult<Public.DTO.v1.ApiResponse<object>>> DeleteRecipe(int id)
        {
            var userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new Public.DTO.v1.ApiResponse<object>
                {
                    Success = false,
                    Message = "User not authenticated"
                });
            }

            try
            {
                var recipe = await _bll.Recipes.FindAsync(id, userId);
                if (recipe == null)
                {
                    return NotFound(new Public.DTO.v1.ApiResponse<object>
                    {
                        Success = false,
                        Message = $"Recipe with ID {id} not found"
                    });
                }

                var result = await _bll.Recipes.RemoveAsync(id, userId);

                return Ok(new Public.DTO.v1.ApiResponse<object>
                {
                    Success = true,
                    Message = "Recipe deleted successfully"
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return BadRequest(new Public.DTO.v1.ApiResponse<object>
                {
                    Success = false,
                    Message = $"Error deleting recipe: {ex.Message}"
                });
            }
        }
    }
}