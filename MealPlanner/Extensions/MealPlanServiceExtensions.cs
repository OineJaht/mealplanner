using AutoMapper;
using MealPlanner.BLL.Services;
using MealPlanner.Data.UnitOfWork;
using MealPlanner.Dto;

namespace MealPlanner.Extensions
{
    public static class MealPlanServiceExtensions
    {
        public static async Task<MealPlanItemDto?> GetItemAsync(
            this IMealPlanService service, 
            int id, 
            string userId,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            // Get the meal plan item
            var mealPlanItem = await unitOfWork.MealPlanItems.GetByIdAsync(id);
            if (mealPlanItem == null)
            {
                return null;
            }
    
            // Verify that the user owns the associated meal plan
            var mealPlan = await unitOfWork.MealPlans.GetByIdAsync(mealPlanItem.MealPlanId);
            if (mealPlan == null || mealPlan.UserId != userId)
            {
                // User does not own this meal plan
                return null;
            }
    
            // Item exists and user is authorized, map to DTO and return
            return mapper.Map<MealPlanItemDto>(mealPlanItem);
        }
    }
}