// MealPlanner/ApiControllers/v1/ShoppingListsController.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MealPlanner.BLL.Services;

namespace MealPlanner.ApiControllers.v1
{
    /// <summary>
    /// API controller for shopping list operations
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ShoppingListsController : BaseApiController
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bll">Business logic layer</param>
        /// <param name="mapper">Mapper</param>
        public ShoppingListsController(IAppBLL bll, IMapper mapper) : base(bll, mapper)
        {
        }

        /// <summary>
        /// Get all shopping lists for currently logged in user
        /// </summary>
        /// <returns>List of shopping lists</returns>
        [HttpGet]
        [ProducesResponseType(typeof(Public.DTO.v1.ApiResponse<IEnumerable<Public.DTO.v1.ShoppingList>>), 200)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<Public.DTO.v1.ApiResponse<IEnumerable<Public.DTO.v1.ShoppingList>>>> GetShoppingLists()
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return Unauthorized(Error<IEnumerable<Public.DTO.v1.ShoppingList>>("User not authenticated"));
            }

            var shoppingLists = await Bll.ShoppingLists.AllAsync(userId);
            var mappedLists = Mapper.Map<IEnumerable<Public.DTO.v1.ShoppingList>>(shoppingLists);
            
            return Ok(Success(mappedLists));
        }
        
        // Add these methods to your existing ShoppingListsController.cs

/// <summary>
/// Update shopping list
/// </summary>
/// <param name="id">Shopping list ID</param>
/// <param name="shoppingList">Updated shopping list data</param>
/// <returns>Updated shopping list</returns>
[HttpPut("{id}")]
[ProducesResponseType(typeof(Public.DTO.v1.ApiResponse<Public.DTO.v1.ShoppingList>), 200)]
[ProducesResponseType(400)]
[ProducesResponseType(404)]
[ProducesResponseType(401)]
[ProducesResponseType(403)]
public async Task<ActionResult<Public.DTO.v1.ApiResponse<Public.DTO.v1.ShoppingList>>> UpdateShoppingList(
    int id, [FromBody] Dto.ShoppingListUpdateDto shoppingList)
{
    if (id != shoppingList.Id)
    {
        return BadRequest(Error<Public.DTO.v1.ShoppingList>("ID mismatch"));
    }

    if (!ModelState.IsValid)
    {
        return BadRequest(Error<Public.DTO.v1.ShoppingList>("Invalid shopping list data"));
    }

    var userId = GetRequiredUserId();

    try
    {
        // Check if shopping list exists
        var existingList = await Bll.ShoppingLists.FindAsync(id, userId);
        if (existingList == null)
        {
            return NotFound(Error<Public.DTO.v1.ShoppingList>($"Shopping list with ID {id} not found"));
        }

        // Map to BLL DTO
        var bllShoppingList = Mapper.Map<BLL.DTO.ShoppingList>(shoppingList);
        
        // Update shopping list
        var updatedList = await Bll.ShoppingLists.UpdateAsync(bllShoppingList, userId);
        
        // Map to API response
        var mappedList = Mapper.Map<Public.DTO.v1.ShoppingList>(updatedList);
        
        return Ok(Success(mappedList, "Shopping list updated successfully"));
    }
    catch (UnauthorizedAccessException)
    {
        return Forbid();
    }
    catch (Exception ex)
    {
        return BadRequest(Error<Public.DTO.v1.ShoppingList>($"Error updating shopping list: {ex.Message}"));
    }
}

/// <summary>
/// Delete shopping list
/// </summary>
/// <param name="id">Shopping list ID</param>
/// <returns>Success response</returns>
[HttpDelete("{id}")]
[ProducesResponseType(typeof(Public.DTO.v1.ApiResponse<object>), 200)]
[ProducesResponseType(404)]
[ProducesResponseType(401)]
[ProducesResponseType(403)]
public async Task<ActionResult<Public.DTO.v1.ApiResponse<object>>> DeleteShoppingList(int id)
{
    var userId = GetRequiredUserId();

    try
    {
        // Check if shopping list exists
        var shoppingList = await Bll.ShoppingLists.FindAsync(id, userId);
        if (shoppingList == null)
        {
            return NotFound(Error<object>($"Shopping list with ID {id} not found"));
        }

        // Delete shopping list
        var result = await Bll.ShoppingLists.RemoveAsync(id, userId);
        
        return Ok(Success(new{}, "Shopping list deleted successfully"));
    }
    catch (UnauthorizedAccessException)
    {
        return Forbid();
    }
    catch (Exception ex)
    {
        return BadRequest(Error<object>($"Error deleting shopping list: {ex.Message}"));
    }
}
        

        /// <summary>
        /// Get shopping list by ID
        /// </summary>
        /// <param name="id">Shopping list ID</param>
        /// <returns>Shopping list details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Public.DTO.v1.ApiResponse<Public.DTO.v1.ShoppingListDetail>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<Public.DTO.v1.ApiResponse<Public.DTO.v1.ShoppingListDetail>>> GetShoppingList(int id)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return Unauthorized(Error<Public.DTO.v1.ShoppingListDetail>("User not authenticated"));
            }

            var shoppingList = await Bll.ShoppingLists.GetWithItemsAsync(id, userId);
            if (shoppingList == null)
            {
                return NotFound(Error<Public.DTO.v1.ShoppingListDetail>($"Shopping list with ID {id} not found"));
            }

            var mappedList = Mapper.Map<Public.DTO.v1.ShoppingListDetail>(shoppingList);
            return Ok(Success(mappedList));
        }

        /// <summary>
        /// Create a new shopping list
        /// </summary>
        /// <param name="shoppingList">Shopping list data</param>
        /// <returns>Created shopping list</returns>
        [HttpPost]
        [ProducesResponseType(typeof(Public.DTO.v1.ApiResponse<Public.DTO.v1.ShoppingList>), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<Public.DTO.v1.ApiResponse<Public.DTO.v1.ShoppingList>>> CreateShoppingList([FromBody] Dto.ShoppingListCreateDto shoppingList)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(Error<Public.DTO.v1.ShoppingList>("Invalid shopping list data"));
            }

            var userId = GetUserId();
            if (userId == null)
            {
                return Unauthorized(Error<Public.DTO.v1.ShoppingList>("User not authenticated"));
            }

            try
            {
                // Map to BLL DTO
                var bllShoppingList = Mapper.Map<BLL.DTO.ShoppingList>(shoppingList);
                
                // Add shopping list
                var createdList = await Bll.ShoppingLists.AddAsync(bllShoppingList, userId);
                
                // Map to API response
                var mappedList = Mapper.Map<Public.DTO.v1.ShoppingList>(createdList);
                
                return CreatedAtAction(nameof(GetShoppingList), new { id = mappedList.Id }, 
                    Success(mappedList, "Shopping list created successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(Error<Public.DTO.v1.ShoppingList>($"Error creating shopping list: {ex.Message}"));
            }
        }

        /// <summary>
        /// Create shopping list from meal plan
        /// </summary>
        /// <param name="request">Request data</param>
        /// <returns>Created shopping list</returns>
        [HttpPost("from-meal-plan")]
        [ProducesResponseType(typeof(Public.DTO.v1.ApiResponse<Public.DTO.v1.ShoppingListDetail>), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<ActionResult<Public.DTO.v1.ApiResponse<Public.DTO.v1.ShoppingListDetail>>> CreateFromMealPlan(
            [FromBody] CreateFromMealPlanRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(Error<Public.DTO.v1.ShoppingListDetail>("Invalid request data"));
            }

            var userId = GetRequiredUserId();

            try
            {
                // Create shopping list from meal plan
                var shoppingList = await Bll.ShoppingLists.CreateFromMealPlanAsync(
                    request.MealPlanId, request.Name, userId);
                
                // Map to API response
                var mappedList = Mapper.Map<Public.DTO.v1.ShoppingListDetail>(shoppingList);
                
                return CreatedAtAction(nameof(GetShoppingList), new { id = mappedList.Id }, 
                    Success(mappedList, "Shopping list created successfully"));
            }
            catch (ArgumentException ex)
            {
                return NotFound(Error<Public.DTO.v1.ShoppingListDetail>(ex.Message));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return BadRequest(Error<Public.DTO.v1.ShoppingListDetail>($"Error creating shopping list: {ex.Message}"));
            }
        }

        /// <summary>
        /// Add item to shopping list
        /// </summary>
        /// <param name="id">Shopping list ID</param>
        /// <param name="item">Shopping list item data</param>
        /// <returns>Created shopping list item</returns>
        [HttpPost("{id}/items")]
        [ProducesResponseType(typeof(Public.DTO.v1.ApiResponse<Public.DTO.v1.ShoppingListItem>), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<ActionResult<Public.DTO.v1.ApiResponse<Public.DTO.v1.ShoppingListItem>>> AddShoppingListItem(
            int id, [FromBody] Dto.ShoppingListItemCreateDto item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(Error<Public.DTO.v1.ShoppingListItem>("Invalid shopping list item data"));
            }

            var userId = GetRequiredUserId();

            try
            {
                // Map to BLL DTO
                var bllItem = Mapper.Map<BLL.DTO.ShoppingListItem>(item);
                
                // Add shopping list item
                var createdItem = await Bll.ShoppingLists.AddItemAsync(id, bllItem, userId);
                
                // Map to API response
                var mappedItem = Mapper.Map<Public.DTO.v1.ShoppingListItem>(createdItem);
                
                return CreatedAtAction(nameof(GetShoppingList), new { id }, 
                    Success(mappedItem, "Shopping list item added successfully"));
            }
            catch (ArgumentException ex)
            {
                return NotFound(Error<Public.DTO.v1.ShoppingListItem>(ex.Message));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return BadRequest(Error<Public.DTO.v1.ShoppingListItem>($"Error adding shopping list item: {ex.Message}"));
            }
        }

        /// <summary>
        /// Toggle shopping list item purchased status
        /// </summary>
        /// <param name="id">Shopping list ID</param>
        /// <param name="itemId">Item ID</param>
        /// <param name="request">Toggle request</param>
        /// <returns>Updated shopping list item</returns>
        [HttpPut("{id}/items/{itemId}/toggle")]
        [ProducesResponseType(typeof(Public.DTO.v1.ApiResponse<Public.DTO.v1.ShoppingListItem>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<ActionResult<Public.DTO.v1.ApiResponse<Public.DTO.v1.ShoppingListItem>>> ToggleShoppingListItem(
            int id, int itemId, [FromBody] ToggleItemRequest request)
        {
            var userId = GetRequiredUserId();

            try
            {
                // Toggle shopping list item
                var updatedItem = await Bll.ShoppingLists.ToggleItemAsync(itemId, request.Purchased, userId);
                
                // Map to API response
                var mappedItem = Mapper.Map<Public.DTO.v1.ShoppingListItem>(updatedItem);
                
                return Ok(Success(mappedItem, "Shopping list item updated successfully"));
            }
            catch (ArgumentException ex)
            {
                return NotFound(Error<Public.DTO.v1.ShoppingListItem>(ex.Message));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return BadRequest(Error<Public.DTO.v1.ShoppingListItem>($"Error updating shopping list item: {ex.Message}"));
            }
        }

        /// <summary>
        /// Remove completed shopping list items
        /// </summary>
        /// <param name="id">Shopping list ID</param>
        /// <returns>Success response with removed count</returns>
        [HttpDelete("{id}/completed-items")]
        [ProducesResponseType(typeof(Public.DTO.v1.ApiResponse<RemoveCompletedItemsResponse>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<ActionResult<Public.DTO.v1.ApiResponse<RemoveCompletedItemsResponse>>> RemoveCompletedItems(int id)
        {
            var userId = GetRequiredUserId();

            try
            {
                // Remove completed items
                var removedCount = await Bll.ShoppingLists.RemoveCompletedItemsAsync(id, userId);
                
                return Ok(Success(new RemoveCompletedItemsResponse { RemovedCount = removedCount }, 
                    $"Removed {removedCount} completed items"));
            }
            catch (ArgumentException ex)
            {
                return NotFound(Error<RemoveCompletedItemsResponse>(ex.Message));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return BadRequest(Error<RemoveCompletedItemsResponse>($"Error removing completed items: {ex.Message}"));
            }
        }
    }

    /// <summary>
    /// Request to create shopping list from meal plan
    /// </summary>
    public class CreateFromMealPlanRequest
    {
        /// <summary>
        /// Meal plan ID
        /// </summary>
        [Required]
        public int MealPlanId { get; set; }
        
        /// <summary>
        /// Shopping list name
        /// </summary>
        [Required]
        [StringLength(255)]
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// Request to toggle shopping list item purchased status
    /// </summary>
    public class ToggleItemRequest
    {
        /// <summary>
        /// New purchased status
        /// </summary>
        public bool Purchased { get; set; }
    }

    /// <summary>
    /// Response for remove completed items operation
    /// </summary>
    public class RemoveCompletedItemsResponse
    {
        /// <summary>
        /// Number of items removed
        /// </summary>
        public int RemovedCount { get; set; }
    }
}