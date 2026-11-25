// SecurityExtensions.cs

using System.Security.Claims;

namespace MealPlanner.Security
{
    public static class SecurityExtensions
    {
        // Check if a user owns a resource
        public static bool OwnsResource<T>(this ClaimsPrincipal user, T resource) 
            where T : class, IUserOwnedResource
        {
            if (resource == null || user == null) return false;
        
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            return !string.IsNullOrEmpty(userId) && userId == resource.UserId;
        }
    
        // Get current user ID with null safety
        public static string? GetUserId(this ClaimsPrincipal user)
        {
            if (user == null) return null;
            return user.FindFirstValue(ClaimTypes.NameIdentifier);
        }
        
        // Get current user ID with validation - throws exception if not found
        public static string GetRequiredUserId(this ClaimsPrincipal user)
        {
            if (user == null)
            {
                throw new InvalidOperationException("User is not authenticated");
            }
            
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                throw new InvalidOperationException("User ID claim not found");
            }
            
            return userId;
        }
    }

    // Interface for user-owned resources
    public interface IUserOwnedResource
    {
        string UserId { get; }
    }
}