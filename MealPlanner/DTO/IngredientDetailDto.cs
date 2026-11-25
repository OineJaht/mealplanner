namespace MealPlanner.Dto
{
    public class IngredientDetailDto : IngredientDto
    {
        public IngredientNutritionDto? Nutrition { get; set; } = new IngredientNutritionDto();
    }
}