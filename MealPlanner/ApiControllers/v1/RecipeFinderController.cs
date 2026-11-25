// MealPlanner/ApiControllers/v1/RecipeFinderController.cs
using System;
using System.Collections.Generic;
using System.Linq;
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
    /// API controller for recipe finder operations
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    public class RecipeFinderController : ControllerBase
    {
        private readonly IAppBLL _bll;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bll">Business logic layer</param>
        /// <param name="mapper">Mapper</param>
        public RecipeFinderController(IAppBLL bll, IMapper mapper)
        {
            _bll = bll;
            _mapper = mapper;
        }

        /// <summary>
        /// Find recipes matching ingredients
        /// </summary>
        /// <param name="request">Search request with ingredient IDs</param>
        /// <returns>List of matching recipes with match percentage</returns>
        [HttpPost("find")]
        [ProducesResponseType(typeof(Public.DTO.v1.ApiResponse<IEnumerable<Public.DTO.v1.RecipeMatch>>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<Public.DTO.v1.ApiResponse<IEnumerable<Public.DTO.v1.RecipeMatch>>>> FindRecipes(
            [FromBody] FindRecipesRequest request)
        {
            if (request.IngredientIds == null || request.IngredientIds.Count == 0)
            {
                return BadRequest(new Public.DTO.v1.ApiResponse<IEnumerable<Public.DTO.v1.RecipeMatch>>
                {
                    Success = false,
                    Message = "No ingredients specified"
                });
            }

            var userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new Public.DTO.v1.ApiResponse<IEnumerable<Public.DTO.v1.RecipeMatch>>
                {
                    Success = false,
                    Message = "User not authenticated"
                });
            }

            try
            {
                var matches = await _bll.RecipeFinder.FindRecipesAsync(userId, request.IngredientIds, request.MatchAll);
                var mappedMatches = _mapper.Map<IEnumerable<Public.DTO.v1.RecipeMatch>>(matches);

                return Ok(new Public.DTO.v1.ApiResponse<IEnumerable<Public.DTO.v1.RecipeMatch>>
                {
                    Success = true,
                    Data = mappedMatches
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Public.DTO.v1.ApiResponse<IEnumerable<Public.DTO.v1.RecipeMatch>>
                {
                    Success = false,
                    Message = $"Error finding recipes: {ex.Message}"
                });
            }
        }

/// <summary>
       /// Get recipe details with available ingredients highlighted
       /// </summary>
       /// <param name="id">Recipe ID</param>
       /// <param name="request">Request with available ingredient IDs</param>
       /// <returns>Recipe details with availability</returns>
       [HttpPost("{id}/availability")]
       [ProducesResponseType(typeof(Public.DTO.v1.ApiResponse<RecipeAvailabilityResponse>), 200)]
       [ProducesResponseType(404)]
       [ProducesResponseType(401)]
       public async Task<ActionResult<Public.DTO.v1.ApiResponse<RecipeAvailabilityResponse>>> GetRecipeWithAvailability(
           int id, [FromBody] AvailabilityRequest request)
       {
           var userId = User.GetUserId();
           if (string.IsNullOrEmpty(userId))
           {
               return Unauthorized(new Public.DTO.v1.ApiResponse<RecipeAvailabilityResponse>
               {
                   Success = false,
                   Message = "User not authenticated"
               });
           }

           try
           {
               var recipe = await _bll.RecipeFinder.GetRecipeWithAvailabilityAsync(
                   id, request.AvailableIngredientIds, userId);
               
               if (recipe == null)
               {
                   return NotFound(new Public.DTO.v1.ApiResponse<RecipeAvailabilityResponse>
                   {
                       Success = false,
                       Message = $"Recipe with ID {id} not found"
                   });
               }
               
               // Create a response from the BLL DTO
               var response = new RecipeAvailabilityResponse
               {
                   Recipe = _mapper.Map<Public.DTO.v1.RecipeDetail>(recipe.Recipe), // Fixed: recipe.Recipe instead of Recipe
                   AvailableIngredients = recipe.RecipeIngredients // Fixed: recipe.RecipeIngredients instead of RecipeIngredients
                       .Where(ri => ri.IsAvailable)
                       .Select(ri => ri.IngredientId)
                       .ToList(),
                   MissingIngredients = recipe.RecipeIngredients // Fixed: recipe.RecipeIngredients instead of RecipeIngredients
                       .Where(ri => !ri.IsAvailable)
                       .Select(ri => ri.IngredientId)
                       .ToList()
               };
               
               return Ok(new Public.DTO.v1.ApiResponse<RecipeAvailabilityResponse>
               {
                   Success = true,
                   Data = response
               });
           }
           catch (Exception ex)
           {
               return BadRequest(new Public.DTO.v1.ApiResponse<RecipeAvailabilityResponse>
               {
                   Success = false,
                   Message = $"Error getting recipe: {ex.Message}"
               });
           }
       }

       /// <summary>
       /// Get pantry ingredients for currently logged in user
       /// </summary>
       /// <returns>List of pantry ingredients</returns>
       [HttpGet("pantry-ingredients")]
       [ProducesResponseType(typeof(Public.DTO.v1.ApiResponse<IEnumerable<Public.DTO.v1.Ingredient>>), 200)]
       [ProducesResponseType(401)]
       public async Task<ActionResult<Public.DTO.v1.ApiResponse<IEnumerable<Public.DTO.v1.Ingredient>>>> GetPantryIngredients()
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

           try
           {
               var ingredients = await _bll.UserPantry.GetUserIngredientsAsync(userId);
               var mappedIngredients = _mapper.Map<IEnumerable<Public.DTO.v1.Ingredient>>(ingredients);
               
               return Ok(new Public.DTO.v1.ApiResponse<IEnumerable<Public.DTO.v1.Ingredient>>
               {
                   Success = true,
                   Data = mappedIngredients
               });
           }
           catch (Exception ex)
           {
               return BadRequest(new Public.DTO.v1.ApiResponse<IEnumerable<Public.DTO.v1.Ingredient>>
               {
                   Success = false,
                   Message = $"Error getting pantry ingredients: {ex.Message}"
               });
           }
       }
   }

   /// <summary>
   /// Request to find recipes by ingredients
   /// </summary>
   public class FindRecipesRequest
   {
       /// <summary>
       /// List of ingredient IDs
       /// </summary>
       public List<int> IngredientIds { get; set; } = new List<int>();
       
       /// <summary>
       /// Whether to match all ingredients (true) or any (false)
       /// </summary>
       public bool MatchAll { get; set; }
   }

   /// <summary>
   /// Request for recipe availability check
   /// </summary>
   public class AvailabilityRequest
   {
       /// <summary>
       /// List of available ingredient IDs
       /// </summary>
       public List<int> AvailableIngredientIds { get; set; } = new List<int>();
   }

   /// <summary>
   /// Response for recipe availability check
   /// </summary>
   public class RecipeAvailabilityResponse
   {
       /// <summary>
       /// Recipe details
       /// </summary>
       public Public.DTO.v1.RecipeDetail? Recipe { get; set; }
       
       /// <summary>
       /// List of available ingredient IDs
       /// </summary>
       public List<int> AvailableIngredients { get; set; } = new List<int>();
       
       /// <summary>
       /// List of missing ingredient IDs
       /// </summary>
       public List<int> MissingIngredients { get; set; } = new List<int>();
   }
}