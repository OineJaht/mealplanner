// MealPlanner/ApiControllers/v1/MealPlansController.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MealPlanner.BLL.Services;

namespace MealPlanner.ApiControllers.v1
{
    /// <summary>
    /// API controller for meal plan operations
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class MealPlansController : BaseApiController
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bll">Business logic layer</param>
        /// <param name="mapper">Mapper</param>
        public MealPlansController(IAppBLL bll, IMapper mapper) : base(bll, mapper)
        {
        }

        /// <summary>
        /// Get all meal plans for currently logged in user
        /// </summary>
        /// <returns>List of meal plans</returns>
        [HttpGet]
        [ProducesResponseType(typeof(Public.DTO.v1.ApiResponse<IEnumerable<Public.DTO.v1.MealPlan>>), 200)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<Public.DTO.v1.ApiResponse<IEnumerable<Public.DTO.v1.MealPlan>>>> GetMealPlans()
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return Unauthorized(Error<IEnumerable<Public.DTO.v1.MealPlan>>("User not authenticated"));
            }

            var mealPlans = await Bll.MealPlans.AllAsync(userId);
            var mappedMealPlans = Mapper.Map<IEnumerable<Public.DTO.v1.MealPlan>>(mealPlans);
            
            return Ok(Success(mappedMealPlans));
        }

        /// <summary>
        /// Get meal plan by ID
        /// </summary>
        /// <param name="id">Meal plan ID</param>
        /// <returns>Meal plan details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Public.DTO.v1.ApiResponse<Public.DTO.v1.MealPlanDetail>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<Public.DTO.v1.ApiResponse<Public.DTO.v1.MealPlanDetail>>> GetMealPlan(int id)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return Unauthorized(Error<Public.DTO.v1.MealPlanDetail>("User not authenticated"));
            }

            // IMPORTANT: Make sure this method includes items
            var mealPlan = await Bll.MealPlans.GetWithItemsAsync(id, userId);
            if (mealPlan == null)
            {
                return NotFound(Error<Public.DTO.v1.MealPlanDetail>($"Meal plan with ID {id} not found"));
            }

            var mappedMealPlan = Mapper.Map<Public.DTO.v1.MealPlanDetail>(mealPlan);
    
            // Add debug logging to see what's being returned
            Console.WriteLine($"DEBUG: GetMealPlan - Original meal plan has {mealPlan.MealPlanItems?.Count ?? 0} items");
            Console.WriteLine($"DEBUG: GetMealPlan - Mapped meal plan has {mappedMealPlan.MealPlanItems?.Count ?? 0} items");
    
            if (mealPlan.MealPlanItems?.Any() == true)
            {
                Console.WriteLine($"DEBUG: First original item: {System.Text.Json.JsonSerializer.Serialize(mealPlan.MealPlanItems.First())}");
            }
    
            if (mappedMealPlan.MealPlanItems?.Any() == true)
            {
                Console.WriteLine($"DEBUG: First mapped item: {System.Text.Json.JsonSerializer.Serialize(mappedMealPlan.MealPlanItems.First())}");
            }
    
            return Ok(Success(mappedMealPlan));
        }

        /// <summary>
        /// Get current meal plan for user
        /// </summary>
        /// <returns>Current meal plan</returns>
        [HttpGet("current")]
        [ProducesResponseType(typeof(Public.DTO.v1.ApiResponse<Public.DTO.v1.MealPlanDetail>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<Public.DTO.v1.ApiResponse<Public.DTO.v1.MealPlanDetail>>> GetCurrentMealPlan()
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return Unauthorized(Error<Public.DTO.v1.MealPlanDetail>("User not authenticated"));
            }

            var mealPlan = await Bll.MealPlans.GetCurrentAsync(userId);
            if (mealPlan == null)
            {
                return NotFound(Error<Public.DTO.v1.MealPlanDetail>("No current meal plan found"));
            }

            var mappedMealPlan = Mapper.Map<Public.DTO.v1.MealPlanDetail>(mealPlan);
            return Ok(Success(mappedMealPlan));
        }

        /// <summary>
        /// Create a new meal plan
        /// </summary>
        /// <param name="mealPlan">Meal plan data</param>
        /// <returns>Created meal plan</returns>
        [HttpPost]
        [ProducesResponseType(typeof(Public.DTO.v1.ApiResponse<Public.DTO.v1.MealPlan>), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<Public.DTO.v1.ApiResponse<Public.DTO.v1.MealPlan>>> CreateMealPlan([FromBody] Dto.MealPlanCreateDto mealPlan)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(Error<Public.DTO.v1.MealPlan>("Invalid meal plan data"));
            }

            var userId = GetUserId();
            if (userId == null)
            {
                return Unauthorized(Error<Public.DTO.v1.MealPlan>("User not authenticated"));
            }

            try
            {
                // Map to BLL DTO
                var bllMealPlan = Mapper.Map<BLL.DTO.MealPlan>(mealPlan);
                
                // Add meal plan
                var createdMealPlan = await Bll.MealPlans.AddAsync(bllMealPlan, userId);
                
                // Map to API response
                var mappedMealPlan = Mapper.Map<Public.DTO.v1.MealPlan>(createdMealPlan);
                
                return CreatedAtAction(nameof(GetMealPlan), new { id = mappedMealPlan.Id }, 
                    Success(mappedMealPlan, "Meal plan created successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(Error<Public.DTO.v1.MealPlan>($"Error creating meal plan: {ex.Message}"));
            }
        }

        /// <summary>
        /// Add item to meal plan
        /// </summary>
        /// <param name="id">Meal plan ID</param>
        /// <param name="item">Meal plan item data</param>
        /// <returns>Created meal plan item</returns>
        [HttpPost("{id}/items")]
        [ProducesResponseType(typeof(Public.DTO.v1.ApiResponse<Public.DTO.v1.MealPlanItem>), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<ActionResult<Public.DTO.v1.ApiResponse<Public.DTO.v1.MealPlanItem>>> AddMealPlanItem(int id, [FromBody] Dto.MealPlanItemCreateDto item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(Error<Public.DTO.v1.MealPlanItem>("Invalid meal plan item data"));
            }

            if (id != item.MealPlanId)
            {
                return BadRequest(Error<Public.DTO.v1.MealPlanItem>("Meal plan ID mismatch"));
            }

            var userId = GetRequiredUserId();

            try
            {
                // Map to BLL DTO
                var bllItem = Mapper.Map<BLL.DTO.MealPlanItem>(item);
                
                // Add meal plan item
                var createdItem = await Bll.MealPlans.AddItemAsync(id, bllItem, userId);
                
                // Map to API response
                var mappedItem = Mapper.Map<Public.DTO.v1.MealPlanItem>(createdItem);
                
                return CreatedAtAction(nameof(GetMealPlan), new { id }, 
                    Success(mappedItem, "Meal plan item added successfully"));
            }
            catch (ArgumentException ex)
            {
                return NotFound(Error<Public.DTO.v1.MealPlanItem>(ex.Message));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return BadRequest(Error<Public.DTO.v1.MealPlanItem>($"Error adding meal plan item: {ex.Message}"));
            }
        }

        /// <summary>
        /// Update meal plan item
        /// </summary>
        /// <param name="id">Meal plan ID</param>
        /// <param name="itemId">Item ID</param>
        /// <param name="item">Updated meal plan item data</param>
        /// <returns>Updated meal plan item</returns>
        [HttpPut("{id}/items/{itemId}")]
        [ProducesResponseType(typeof(Public.DTO.v1.ApiResponse<Public.DTO.v1.MealPlanItem>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<ActionResult<Public.DTO.v1.ApiResponse<Public.DTO.v1.MealPlanItem>>> UpdateMealPlanItem(
            int id, int itemId, [FromBody] Dto.MealPlanItemUpdateDto item)
        {
            if (itemId != item.Id)
            {
                return BadRequest(Error<Public.DTO.v1.MealPlanItem>("Item ID mismatch"));
            }

            if (id != item.MealPlanId)
            {
                return BadRequest(Error<Public.DTO.v1.MealPlanItem>("Meal plan ID mismatch"));
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(Error<Public.DTO.v1.MealPlanItem>("Invalid meal plan item data"));
            }

            var userId = GetRequiredUserId();

            try
            {
                // Map to BLL DTO
                var bllItem = Mapper.Map<BLL.DTO.MealPlanItem>(item);
                
                // Update meal plan item
                var updatedItem = await Bll.MealPlans.UpdateItemAsync(bllItem, userId);
                
                // Map to API response
                var mappedItem = Mapper.Map<Public.DTO.v1.MealPlanItem>(updatedItem);
                
                return Ok(Success(mappedItem, "Meal plan item updated successfully"));
            }
            catch (ArgumentException ex)
            {
                return NotFound(Error<Public.DTO.v1.MealPlanItem>(ex.Message));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return BadRequest(Error<Public.DTO.v1.MealPlanItem>($"Error updating meal plan item: {ex.Message}"));
            }
        }

        /// <summary>
        /// Delete meal plan item
        /// </summary>
        /// <param name="id">Meal plan ID</param>
        /// <param name="itemId">Item ID</param>
        /// <returns>Success response</returns>
        [HttpDelete("{id}/items/{itemId}")]
        [ProducesResponseType(typeof(Public.DTO.v1.ApiResponse<object>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<ActionResult<Public.DTO.v1.ApiResponse<object>>> DeleteMealPlanItem(int id, int itemId)
        {
            var userId = GetRequiredUserId();

            try
            {
                // Delete meal plan item
                var result = await Bll.MealPlans.RemoveItemAsync(itemId, userId);
                
                if (!result)
                {
                    return NotFound(Error<object>($"Meal plan item with ID {itemId} not found"));
                }
                
                return Ok(Success(new{}, "Meal plan item deleted successfully"));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return BadRequest(Error<object>($"Error deleting meal plan item: {ex.Message}"));
            }
        }

        /// <summary>
        /// Update meal plan
        /// </summary>
        /// <param name="id">Meal plan ID</param>
        /// <param name="mealPlan">Updated meal plan data</param>
        /// <returns>Updated meal plan</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Public.DTO.v1.ApiResponse<Public.DTO.v1.MealPlan>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<ActionResult<Public.DTO.v1.ApiResponse<Public.DTO.v1.MealPlan>>> UpdateMealPlan(
            int id, [FromBody] Dto.MealPlanUpdateDto mealPlan)
        {
            if (id != mealPlan.Id)
            {
                return BadRequest(Error<Public.DTO.v1.MealPlan>("ID mismatch"));
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(Error<Public.DTO.v1.MealPlan>("Invalid meal plan data"));
            }

            var userId = GetRequiredUserId();

            try
            {
                // Check if meal plan exists
                var existingMealPlan = await Bll.MealPlans.FindAsync(id, userId);
                if (existingMealPlan == null)
                {
                    return NotFound(Error<Public.DTO.v1.MealPlan>($"Meal plan with ID {id} not found"));
                }

                // Map to BLL DTO
                var bllMealPlan = Mapper.Map<BLL.DTO.MealPlan>(mealPlan);
                
                // Update meal plan
                var updatedMealPlan = await Bll.MealPlans.UpdateAsync(bllMealPlan, userId);
                
                // Map to API response
                var mappedMealPlan = Mapper.Map<Public.DTO.v1.MealPlan>(updatedMealPlan);
                
                return Ok(Success(mappedMealPlan, "Meal plan updated successfully"));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return BadRequest(Error<Public.DTO.v1.MealPlan>($"Error updating meal plan: {ex.Message}"));
            }
        }

        /// <summary>
        /// Delete meal plan
        /// </summary>
        /// <param name="id">Meal plan ID</param>
        /// <returns>Success response</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(Public.DTO.v1.ApiResponse<object>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<ActionResult<Public.DTO.v1.ApiResponse<object>>> DeleteMealPlan(int id)
        {
            var userId = GetRequiredUserId();

            try
            {
                // Check if meal plan exists
                var mealPlan = await Bll.MealPlans.FindAsync(id, userId);
                if (mealPlan == null)
                {
                    return NotFound(Error<object>($"Meal plan with ID {id} not found"));
                }

                // Delete meal plan
                var result = await Bll.MealPlans.RemoveAsync(id, userId);
                
                return Ok(Success(new{}, "Meal plan deleted successfully"));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return BadRequest(Error<object>($"Error deleting meal plan: {ex.Message}"));
            }
        }
    }
}