// MealPlanner/ApiControllers/BaseApiController.cs
using AutoMapper;
using MealPlanner.BLL.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MealPlanner.Security;

namespace MealPlanner.ApiControllers
{
    /// <summary>
    /// Base controller for all API controllers
    /// </summary>
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    public abstract class BaseApiController : ControllerBase
    {
        /// <summary>
        /// Business logic layer
        /// </summary>
        protected readonly IAppBLL Bll;
        
        /// <summary>
        /// Mapper
        /// </summary>
        protected readonly IMapper Mapper;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bll">Business logic layer</param>
        /// <param name="mapper">Mapper</param>
        protected BaseApiController(IAppBLL bll, IMapper mapper)
        {
            Bll = bll;
            Mapper = mapper;
        }

        /// <summary>
        /// Get current user ID or null if not authenticated
        /// </summary>
        /// <returns>User ID or null</returns>
        protected string? GetUserId()
        {
            return User.GetUserId();
        }

        /// <summary>
        /// Get current user ID or throw exception if not authenticated
        /// </summary>
        /// <returns>User ID</returns>
        /// <exception cref="System.InvalidOperationException">Thrown if user is not authenticated</exception>
        protected string GetRequiredUserId()
        {
            return User.GetRequiredUserId();
        }

        /// <summary>
        /// Create success response
        /// </summary>
        /// <typeparam name="T">Response data type</typeparam>
        /// <param name="data">Response data</param>
        /// <param name="message">Response message</param>
        /// <returns>Success response</returns>
        protected Public.DTO.v1.ApiResponse<T> Success<T>(T data, string message = "Success")
        {
            return new Public.DTO.v1.ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        /// <summary>
        /// Create error response
        /// </summary>
        /// <typeparam name="T">Response data type</typeparam>
        /// <param name="message">Error message</param>
        /// <param name="data">Optional data</param>
        /// <returns>Error response</returns>
        protected Public.DTO.v1.ApiResponse<T> Error<T>(string message, T? data = default)
        {
            return new Public.DTO.v1.ApiResponse<T>
            {
                Success = false,
                Message = message,
                Data = data
            };
        }
    }
}