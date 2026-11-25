// MealPlanner/ApiControllers/v1/IngredientsController.cs
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
    /// API controller for ingredient operations
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    public class IngredientsController : ControllerBase
    {
        private readonly IAppBLL _bll;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bll">Business logic layer</param>
        /// <param name="mapper">Mapper</param>
        public IngredientsController(IAppBLL bll, IMapper mapper)
        {
            _bll = bll;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all ingredients
        /// </summary>
        /// <returns>List of ingredients</returns>
        [HttpGet]
        [ProducesResponseType(typeof(Public.DTO.v1.ApiResponse<IEnumerable<Public.DTO.v1.Ingredient>>), 200)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<Public.DTO.v1.ApiResponse<IEnumerable<Public.DTO.v1.Ingredient>>>> GetIngredients()
        {
            var userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new Public.DTO.v1.ApiResponse<IEnumerable<Public.DTO.v1.Ingredient>>
                {
                    Success = false,
                    Message = "User not authenticated"
                });
            }

            var data = await _bll.Ingredients.AllAsync();
            var ingredients = _mapper.Map<IEnumerable<Public.DTO.v1.Ingredient>>(data);

            return Ok(new Public.DTO.v1.ApiResponse<IEnumerable<Public.DTO.v1.Ingredient>>
            {
                Success = true,
                Data = ingredients
            });
        }

        /// <summary>
        /// Get ingredient by ID
        /// </summary>
        /// <param name="id">Ingredient ID</param>
        /// <returns>Ingredient details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Public.DTO.v1.ApiResponse<Public.DTO.v1.IngredientDetail>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<Public.DTO.v1.ApiResponse<Public.DTO.v1.IngredientDetail>>> GetIngredient(int id)
        {
            var userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new Public.DTO.v1.ApiResponse<Public.DTO.v1.IngredientDetail>
                {
                    Success = false,
                    Message = "User not authenticated"
                });
            }

            var ingredient = await _bll.Ingredients.GetWithNutritionAsync(id);
            if (ingredient == null)
            {
                return NotFound(new Public.DTO.v1.ApiResponse<Public.DTO.v1.IngredientDetail>
                {
                    Success = false,
                    Message = $"Ingredient with ID {id} not found"
                });
            }

            var ingredientDetail = _mapper.Map<Public.DTO.v1.IngredientDetail>(ingredient);

            return Ok(new Public.DTO.v1.ApiResponse<Public.DTO.v1.IngredientDetail>
            {
                Success = true,
                Data = ingredientDetail
            });
        }

        /// <summary>
        /// Search for ingredients
        /// </summary>
        /// <param name="term">Search term</param>
        /// <returns>List of matching ingredients</returns>
        [HttpGet("search")]
        [ProducesResponseType(typeof(Public.DTO.v1.ApiResponse<IEnumerable<Public.DTO.v1.Ingredient>>), 200)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<Public.DTO.v1.ApiResponse<IEnumerable<Public.DTO.v1.Ingredient>>>> SearchIngredients([FromQuery] string term)
        {
            var userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new Public.DTO.v1.ApiResponse<IEnumerable<Public.DTO.v1.Ingredient>>
                {
                    Success = false,
                    Message = "User not authenticated"
                });
            }

            if (string.IsNullOrWhiteSpace(term))
            {
                return BadRequest(new Public.DTO.v1.ApiResponse<IEnumerable<Public.DTO.v1.Ingredient>>
                {
                    Success = false,
                    Message = "Search term cannot be empty"
                });
            }

            var ingredients = await _bll.Ingredients.SearchAsync(term);
            var result = _mapper.Map<IEnumerable<Public.DTO.v1.Ingredient>>(ingredients);

            return Ok(new Public.DTO.v1.ApiResponse<IEnumerable<Public.DTO.v1.Ingredient>>
            {
                Success = true,
                Data = result
            });
        }

        /// <summary>
        /// Get ingredients by category
        /// </summary>
        /// <param name="category">Category name</param>
        /// <returns>List of ingredients in category</returns>
        [HttpGet("category/{category}")]
        [ProducesResponseType(typeof(Public.DTO.v1.ApiResponse<IEnumerable<Public.DTO.v1.Ingredient>>), 200)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<Public.DTO.v1.ApiResponse<IEnumerable<Public.DTO.v1.Ingredient>>>> GetIngredientsByCategory(string category)
        {
            var userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new Public.DTO.v1.ApiResponse<IEnumerable<Public.DTO.v1.Ingredient>>
                {
                    Success = false,
                    Message = "User not authenticated"
                });
            }

            if (string.IsNullOrWhiteSpace(category))
            {
                return BadRequest(new Public.DTO.v1.ApiResponse<IEnumerable<Public.DTO.v1.Ingredient>>
                {
                    Success = false,
                    Message = "Category name cannot be empty"
                });
            }

            var ingredients = await _bll.Ingredients.GetByCategoryAsync(category);
            var result = _mapper.Map<IEnumerable<Public.DTO.v1.Ingredient>>(ingredients);

            return Ok(new Public.DTO.v1.ApiResponse<IEnumerable<Public.DTO.v1.Ingredient>>
            {
                Success = true,
                Data = result
            });
        }

        /// <summary>
        /// Create a new ingredient
        /// </summary>
        /// <param name="ingredient">Ingredient data</param>
        /// <returns>Created ingredient</returns>
        [HttpPost]
        [ProducesResponseType(typeof(Public.DTO.v1.ApiResponse<Public.DTO.v1.Ingredient>), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<Public.DTO.v1.ApiResponse<Public.DTO.v1.Ingredient>>> CreateIngredient([FromBody] MealPlanner.Dto.IngredientCreateDto ingredient)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Public.DTO.v1.ApiResponse<Public.DTO.v1.Ingredient>
                {
                    Success = false,
                    Message = "Invalid ingredient data"
                });
            }

            var userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new Public.DTO.v1.ApiResponse<Public.DTO.v1.Ingredient>
                {
                    Success = false,
                    Message = "User not authenticated"
                });
            }

            try
            {
                var bllIngredient = _mapper.Map<BLL.DTO.Ingredient>(ingredient);
                var createdIngredient = await _bll.Ingredients.AddAsync(bllIngredient, userId);
                var result = _mapper.Map<Public.DTO.v1.Ingredient>(createdIngredient);

                return CreatedAtAction(nameof(GetIngredient), new { id = result.Id }, new Public.DTO.v1.ApiResponse<Public.DTO.v1.Ingredient>
                {
                    Success = true,
                    Message = "Ingredient created successfully",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Public.DTO.v1.ApiResponse<Public.DTO.v1.Ingredient>
                {
                    Success = false,
                    Message = $"Error creating ingredient: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Update an existing ingredient
        /// </summary>
        /// <param name="id">Ingredient ID</param>
        /// <param name="ingredient">Updated ingredient data</param>
        /// <returns>Updated ingredient</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Public.DTO.v1.ApiResponse<Public.DTO.v1.Ingredient>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<Public.DTO.v1.ApiResponse<Public.DTO.v1.Ingredient>>> UpdateIngredient(int id, [FromBody] MealPlanner.Dto.IngredientUpdateDto ingredient)
        {
            if (id != ingredient.Id)
            {
                return BadRequest(new Public.DTO.v1.ApiResponse<Public.DTO.v1.Ingredient>
                {
                    Success = false,
                    Message = "ID mismatch"
                });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new Public.DTO.v1.ApiResponse<Public.DTO.v1.Ingredient>
                {
                    Success = false,
                    Message = "Invalid ingredient data"
                });
            }

            var userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new Public.DTO.v1.ApiResponse<Public.DTO.v1.Ingredient>
                {
                    Success = false,
                    Message = "User not authenticated"
                });
            }

            try
            {
                var existingIngredient = await _bll.Ingredients.FindAsync(id);
                if (existingIngredient == null)
                {
                    return NotFound(new Public.DTO.v1.ApiResponse<Public.DTO.v1.Ingredient>
                    {
                        Success = false,
                        Message = $"Ingredient with ID {id} not found"
                    });
                }

                var bllIngredient = _mapper.Map<BLL.DTO.Ingredient>(ingredient);
                var updatedIngredient = await _bll.Ingredients.UpdateAsync(bllIngredient, userId);
                var result = _mapper.Map<Public.DTO.v1.Ingredient>(updatedIngredient);

                return Ok(new Public.DTO.v1.ApiResponse<Public.DTO.v1.Ingredient>
                {
                    Success = true,
                    Message = "Ingredient updated successfully",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Public.DTO.v1.ApiResponse<Public.DTO.v1.Ingredient>
                {
                    Success = false,
                    Message = $"Error updating ingredient: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Delete an ingredient
        /// </summary>
        /// <param name="id">Ingredient ID</param>
        /// <returns>Success response</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(Public.DTO.v1.ApiResponse<object>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<Public.DTO.v1.ApiResponse<object>>> DeleteIngredient(int id)
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
                var ingredient = await _bll.Ingredients.FindAsync(id);
                if (ingredient == null)
                {
                    return NotFound(new Public.DTO.v1.ApiResponse<object>
                    {
                        Success = false,
                        Message = $"Ingredient with ID {id} not found"
                    });
                }

                var result = await _bll.Ingredients.RemoveAsync(id, userId);

                return Ok(new Public.DTO.v1.ApiResponse<object>
                {
                    Success = true,
                    Message = "Ingredient deleted successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Public.DTO.v1.ApiResponse<object>
                {
                    Success = false,
                    Message = $"Error deleting ingredient: {ex.Message}"
                });
            }
        }
    }
}