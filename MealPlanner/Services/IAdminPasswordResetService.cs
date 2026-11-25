namespace MealPlanner.Services
{
    public interface IAdminPasswordResetService
    {
        Task<bool> ResetUserPasswordAsync(string userId, string newPassword);
        Task<bool> ForcePasswordChangeAsync(string userId);
    }
}