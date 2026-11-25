using Microsoft.AspNetCore.Identity;
using MealPlanner.Models.Inventory;

namespace MealPlanner.Services
{
    public class AdminPasswordResetService : IAdminPasswordResetService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AdminPasswordResetService> _logger;

        public AdminPasswordResetService(
            UserManager<ApplicationUser> userManager,
            ILogger<AdminPasswordResetService> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<bool> ResetUserPasswordAsync(string userId, string newPassword)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning($"User with ID {userId} not found for password reset");
                    return false;
                }

                // Remove existing password and set new one
                var removePasswordResult = await _userManager.RemovePasswordAsync(user);
                if (!removePasswordResult.Succeeded)
                {
                    _logger.LogError($"Failed to remove password for user {userId}: {string.Join(", ", removePasswordResult.Errors.Select(e => e.Description))}");
                    return false;
                }

                var addPasswordResult = await _userManager.AddPasswordAsync(user, newPassword);
                if (!addPasswordResult.Succeeded)
                {
                    _logger.LogError($"Failed to set new password for user {userId}: {string.Join(", ", addPasswordResult.Errors.Select(e => e.Description))}");
                    return false;
                }

                _logger.LogInformation($"Password reset successfully for user {userId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error resetting password for user {userId}");
                return false;
            }
        }

        public async Task<bool> ForcePasswordChangeAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning($"User with ID {userId} not found for force password change");
                    return false;
                }

                // Set security stamp to invalidate existing tokens/cookies
                await _userManager.UpdateSecurityStampAsync(user);

                _logger.LogInformation($"Forced password change for user {userId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error forcing password change for user {userId}");
                return false;
            }
        }
    }
}