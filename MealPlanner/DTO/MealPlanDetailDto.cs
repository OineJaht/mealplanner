namespace MealPlanner.Dto
{
    public class MealPlanDetailDto : MealPlanDto
    {
        public List<MealPlanItemDto> MealPlanItems { get; set; } = new List<MealPlanItemDto>();
    }
}