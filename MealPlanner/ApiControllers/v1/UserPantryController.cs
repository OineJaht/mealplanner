// MealPlanner/ApiControllers/v1/UserPantryController.cs
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
    /// API controller for user pantry operations
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    public class UserPantryController : ControllerBase
    {
        private readonly IAppBLL _bll;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bll">Business logic layer</param>
        /// <param name="mapper">Mapper</param>
        public UserPantryController(IAppBLL bll, IMapper mapper)
        {
            _bll = bll;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all pantry items for the current user
        /// </summary>
        /// <returns>List of pantry items</returns>
        [HttpGet]
        [ProducesResponseType(typeof(Public.DTO.v1.ApiResponse<IEnumerable<Public.DTO.v1.UserPantryItem>>), 200)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<Public.DTO.v1.ApiResponse<IEnumerable<Public.DTO.v1.UserPantryItem>>>> GetUserPantryItems()
        {
            var userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new Public.DTO.v1.ApiResponse<IEnumerable<Public.DTO.v1.UserPantryItem>>
                {
                    Success = false,
                    Message = "User not authenticated"
                });
            }

            var pantryItems = await _bll.UserPantry.GetUserItemsAsync(userId);
            var mappedItems = _mapper.Map<IEnumerable<Public.DTO.v1.UserPantryItem>>(pantryItems);

            return Ok(new Public.DTO.v1.ApiResponse<IEnumerable<Public.DTO.v1.UserPantryItem>>
            {
                Success = true,
                Data = mappedItems
            });
        }

        /// <summary>
        /// Get pantry item by ID
        /// </summary>
        /// <param name="id">Pantry item ID</param>
        /// <returns>Pantry item details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Public.DTO.v1.ApiResponse<Public.DTO.v1.UserPantryItem>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<ActionResult<Public.DTO.v1.ApiResponse<Public.DTO.v1.UserPantryItem>>> GetUserPantryItem(int id)
        {
            var userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new Public.DTO.v1.ApiResponse<Public.DTO.v1.UserPantryItem>
                {
                    Success = false,
                    Message = "User not authenticated"
                });
            }

            var pantryItem = await _bll.UserPantry.FindAsync(id, userId);
            if (pantryItem == null)
            {
                return NotFound(new Public.DTO.v1.ApiResponse<Public.DTO.v1.UserPantryItem>
                {
                    Success = false,
                    Message = $"Pantry item with ID {id} not found"
                });
            }

            var mappedItem = _mapper.Map<Public.DTO.v1.UserPantryItem>(pantryItem);

            return Ok(new Public.DTO.v1.ApiResponse<Public.DTO.v1.UserPantryItem>
            {
                Success = true,
                Data = mappedItem
            });
        }

        /// <summary>
        /// Get expiring items in user's pantry
        /// </summary>
        /// <param name="daysThreshold">Days until expiration (default: 7)</param>
        /// <returns>List of expiring pantry items</returns>
        [HttpGet("expiring")]
        [ProducesResponseType(typeof(Public.DTO.v1.ApiResponse<IEnumerable<Public.DTO.v1.UserPantryItem>>), 200)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<Public.DTO.v1.ApiResponse<IEnumerable<Public.DTO.v1.UserPantryItem>>>> GetExpiringItems([FromQuery] int daysThreshold = 7)
        {
            var userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new Public.DTO.v1.ApiResponse<IEnumerable<Public.DTO.v1.UserPantryItem>>
                {
                    Success = false,
                    Message = "User not authenticated"
                });
            }

            var expiringItems = await _bll.UserPantry.GetExpiringItemsAsync(userId, daysThreshold);
            var mappedItems = _mapper.Map<IEnumerable<Public.DTO.v1.UserPantryItem>>(expiringItems);

            return Ok(new Public.DTO.v1.ApiResponse<IEnumerable<Public.DTO.v1.UserPantryItem>>
            {
                Success = true,
                Data = mappedItems
            });
        }

        /// <summary>
        /// Get low stock items in user's pantry
        /// </summary>
        /// <param name="threshold">Amount threshold (default: 5)</param>
        /// <returns>List of low stock pantry items</returns>
        [HttpGet("low-stock")]
        [ProducesResponseType(typeof(Public.DTO.v1.ApiResponse<IEnumerable<Public.DTO.v1.UserPantryItem>>), 200)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<Public.DTO.v1.ApiResponse<IEnumerable<Public.DTO.v1.UserPantryItem>>>> GetLowStockItems([FromQuery] decimal threshold = 5)
        {
            var userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new Public.DTO.v1.ApiResponse<IEnumerable<Public.DTO.v1.UserPantryItem>>
                {
                    Success = false,
                    Message = "User not authenticated"
                });
            }

            var lowStockItems = await _bll.UserPantry.GetLowStockItemsAsync(userId, threshold);
            var mappedItems = _mapper.Map<IEnumerable<Public.DTO.v1.UserPantryItem>>(lowStockItems);

            return Ok(new Public.DTO.v1.ApiResponse<IEnumerable<Public.DTO.v1.UserPantryItem>>
            {
                Success = true,
                Data = mappedItems
            });
        }

        /// <summary>
        /// Add item to user's pantry
        /// </summary>
        /// <param name="item">Pantry item data</param>
        /// <returns>Created pantry item</returns>
        [HttpPost]
        [ProducesResponseType(typeof(Public.DTO.v1.ApiResponse<Public.DTO.v1.UserPantryItem>), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<Public.DTO.v1.ApiResponse<Public.DTO.v1.UserPantryItem>>> AddPantryItem([FromBody] MealPlanner.Dto.UserPantryItemCreateDto item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Public.DTO.v1.ApiResponse<Public.DTO.v1.UserPantryItem>
                {
                    Success = false,
                    Message = "Invalid pantry item data"
                });
            }

            var userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new Public.DTO.v1.ApiResponse<Public.DTO.v1.UserPantryItem>
                {
                    Success = false,
                    Message = "User not authenticated"
                });
            }

            try
            {
                var bllItem = _mapper.Map<BLL.DTO.UserPantryItem>(item);
                var createdItem = await _bll.UserPantry.AddAsync(bllItem, userId);
                var result = _mapper.Map<Public.DTO.v1.UserPantryItem>(createdItem);

                return CreatedAtAction(nameof(GetUserPantryItem), new { id = result.Id }, new Public.DTO.v1.ApiResponse<Public.DTO.v1.UserPantryItem>
                {
                    Success = true,
                    Message = "Pantry item added successfully",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Public.DTO.v1.ApiResponse<Public.DTO.v1.UserPantryItem>
                {
                    Success = false,
                    Message = $"Error adding pantry item: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Update pantry item
        /// </summary>
        /// <param name="id">Pantry item ID</param>
        /// <param name="item">Updated pantry item data</param>
        /// <returns>Updated pantry item</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Public.DTO.v1.ApiResponse<Public.DTO.v1.UserPantryItem>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<ActionResult<Public.DTO.v1.ApiResponse<Public.DTO.v1.UserPantryItem>>> UpdatePantryItem(
            int id, [FromBody] MealPlanner.Dto.UserPantryItemUpdateDto item)
        {
            if (id != item.Id)
            {
                return BadRequest(new Public.DTO.v1.ApiResponse<Public.DTO.v1.UserPantryItem>
                {
                    Success = false,
                    Message = "ID mismatch"
                });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new Public.DTO.v1.ApiResponse<Public.DTO.v1.UserPantryItem>
                {
                    Success = false,
                    Message = "Invalid pantry item data"
                });
            }

            var userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new Public.DTO.v1.ApiResponse<Public.DTO.v1.UserPantryItem>
                {
                    Success = false,
                    Message = "User not authenticated"
                });
            }

            try
            {
                var existingItem = await _bll.UserPantry.FindAsync(id, userId);
                if (existingItem == null)
                {
                    return NotFound(new Public.DTO.v1.ApiResponse<Public.DTO.v1.UserPantryItem>
                    {
                        Success = false,
                        Message = $"Pantry item with ID {id} not found"
                    });
                }

                var bllItem = _mapper.Map<BLL.DTO.UserPantryItem>(item);
                var updatedItem = await _bll.UserPantry.UpdateAsync(bllItem, userId);
                var result = _mapper.Map<Public.DTO.v1.UserPantryItem>(updatedItem);

                return Ok(new Public.DTO.v1.ApiResponse<Public.DTO.v1.UserPantryItem>
                {
                    Success = true,
                    Message = "Pantry item updated successfully",
                    Data = result
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return BadRequest(new Public.DTO.v1.ApiResponse<Public.DTO.v1.UserPantryItem>
                {
                    Success = false,
                    Message = $"Error updating pantry item: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Delete pantry item
        /// </summary>
        /// <param name="id">Pantry item ID</param>
        /// <returns>Success response</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(Public.DTO.v1.ApiResponse<object>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<ActionResult<Public.DTO.v1.ApiResponse<object>>> DeletePantryItem(int id)
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
                var item = await _bll.UserPantry.FindAsync(id, userId);
                if (item == null)
                {
                    return NotFound(new Public.DTO.v1.ApiResponse<object>
                    {
                        Success = false,
                        Message = $"Pantry item with ID {id} not found"
                    });
                }

                var result = await _bll.UserPantry.RemoveAsync(id, userId);

                return Ok(new Public.DTO.v1.ApiResponse<object>
                {
                    Success = true,
                    Message = "Pantry item deleted successfully"
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
                    Message = $"Error deleting pantry item: {ex.Message}"
                });
            }
        }
    }
}