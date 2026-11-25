// MealPlanner/ApiControllers/v1/CategoriesController.cs
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
    /// API controller for category operations
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    public class CategoriesController : ControllerBase
    {
        private readonly IAppBLL _bll;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bll">Business logic layer</param>
        /// <param name="mapper">Mapper</param>
        public CategoriesController(IAppBLL bll, IMapper mapper)
        {
            _bll = bll;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all categories
        /// </summary>
        /// <returns>List of categories</returns>
        [HttpGet]
        [ProducesResponseType(typeof(Public.DTO.v1.ApiResponse<IEnumerable<Public.DTO.v1.Category>>), 200)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<Public.DTO.v1.ApiResponse<IEnumerable<Public.DTO.v1.Category>>>> GetCategories()
        {
            var userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new Public.DTO.v1.ApiResponse<IEnumerable<Public.DTO.v1.Category>>
                {
                    Success = false,
                    Message = "User not authenticated"
                });
            }

            var categories = await _bll.Categories.AllAsync();
            var mappedCategories = _mapper.Map<IEnumerable<Public.DTO.v1.Category>>(categories);

            return Ok(new Public.DTO.v1.ApiResponse<IEnumerable<Public.DTO.v1.Category>>
            {
                Success = true,
                Data = mappedCategories
            });
        }

        /// <summary>
        /// Get all category names
        /// </summary>
        /// <returns>List of category names</returns>
        [HttpGet("names")]
        [ProducesResponseType(typeof(Public.DTO.v1.ApiResponse<IEnumerable<string>>), 200)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<Public.DTO.v1.ApiResponse<IEnumerable<string>>>> GetCategoryNames()
        {
            var userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new Public.DTO.v1.ApiResponse<IEnumerable<string>>
                {
                    Success = false,
                    Message = "User not authenticated"
                });
            }

            var categoryNames = await _bll.Categories.GetAllNamesAsync();

            return Ok(new Public.DTO.v1.ApiResponse<IEnumerable<string>>
            {
                Success = true,
                Data = categoryNames
            });
        }

        /// <summary>
        /// Get category by ID
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <returns>Category details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Public.DTO.v1.ApiResponse<Public.DTO.v1.Category>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<Public.DTO.v1.ApiResponse<Public.DTO.v1.Category>>> GetCategory(int id)
        {
            var userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new Public.DTO.v1.ApiResponse<Public.DTO.v1.Category>
                {
                    Success = false,
                    Message = "User not authenticated"
                });
            }

            var category = await _bll.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound(new Public.DTO.v1.ApiResponse<Public.DTO.v1.Category>
                {
                    Success = false,
                    Message = $"Category with ID {id} not found"
                });
            }

            var mappedCategory = _mapper.Map<Public.DTO.v1.Category>(category);

            return Ok(new Public.DTO.v1.ApiResponse<Public.DTO.v1.Category>
            {
                Success = true,
                Data = mappedCategory
            });
        }

        /// <summary>
        /// Create a new category
        /// </summary>
        /// <param name="category">Category data</param>
        /// <returns>Created category</returns>
        [HttpPost]
        [ProducesResponseType(typeof(Public.DTO.v1.ApiResponse<Public.DTO.v1.Category>), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<Public.DTO.v1.ApiResponse<Public.DTO.v1.Category>>> CreateCategory([FromBody] MealPlanner.Dto.CategoryCreateDto category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Public.DTO.v1.ApiResponse<Public.DTO.v1.Category>
                {
                    Success = false,
                    Message = "Invalid category data"
                });
            }

            var userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new Public.DTO.v1.ApiResponse<Public.DTO.v1.Category>
                {
                    Success = false,
                    Message = "User not authenticated"
                });
            }

            try
            {
                var bllCategory = _mapper.Map<BLL.DTO.Category>(category);
                var createdCategory = await _bll.Categories.AddAsync(bllCategory, userId);
                var result = _mapper.Map<Public.DTO.v1.Category>(createdCategory);

                return CreatedAtAction(nameof(GetCategory), new { id = result.Id }, new Public.DTO.v1.ApiResponse<Public.DTO.v1.Category>
                {
                    Success = true,
                    Message = "Category created successfully",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Public.DTO.v1.ApiResponse<Public.DTO.v1.Category>
                {
                    Success = false,
                    Message = $"Error creating category: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Update an existing category
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <param name="category">Updated category data</param>
        /// <returns>Updated category</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Public.DTO.v1.ApiResponse<Public.DTO.v1.Category>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<Public.DTO.v1.ApiResponse<Public.DTO.v1.Category>>> UpdateCategory(
            int id, [FromBody] MealPlanner.Dto.CategoryUpdateDto category)
        {
            if (id != category.Id)
            {
                return BadRequest(new Public.DTO.v1.ApiResponse<Public.DTO.v1.Category>
                {
                    Success = false,
                    Message = "ID mismatch"
                });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new Public.DTO.v1.ApiResponse<Public.DTO.v1.Category>
                {
                    Success = false,
                    Message = "Invalid category data"
                });
            }

            var userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new Public.DTO.v1.ApiResponse<Public.DTO.v1.Category>
                {
                    Success = false,
                    Message = "User not authenticated"
                });
            }

            try
            {
                var existingCategory = await _bll.Categories.FindAsync(id);
                if (existingCategory == null)
                {
                    return NotFound(new Public.DTO.v1.ApiResponse<Public.DTO.v1.Category>
                    {
                        Success = false,
                        Message = $"Category with ID {id} not found"
                    });
                }

                var bllCategory = _mapper.Map<BLL.DTO.Category>(category);
                var updatedCategory = await _bll.Categories.UpdateAsync(bllCategory, userId);
                var result = _mapper.Map<Public.DTO.v1.Category>(updatedCategory);

                return Ok(new Public.DTO.v1.ApiResponse<Public.DTO.v1.Category>
                {
                    Success = true,
                    Message = "Category updated successfully",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Public.DTO.v1.ApiResponse<Public.DTO.v1.Category>
                {
                    Success = false,
                    Message = $"Error updating category: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Delete a category
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <returns>Success response</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(Public.DTO.v1.ApiResponse<object>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<Public.DTO.v1.ApiResponse<object>>> DeleteCategory(int id)
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
                var category = await _bll.Categories.FindAsync(id);
                if (category == null)
                {
                    return NotFound(new Public.DTO.v1.ApiResponse<object>
                    {
                        Success = false,
                        Message = $"Category with ID {id} not found"
                    });
                }

                var result = await _bll.Categories.RemoveAsync(id, userId);

                return Ok(new Public.DTO.v1.ApiResponse<object>
                {
                    Success = true,
                    Message = "Category deleted successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Public.DTO.v1.ApiResponse<object>
                {
                    Success = false,
                    Message = $"Error deleting category: {ex.Message}"
                });
            }
        }
    }
}