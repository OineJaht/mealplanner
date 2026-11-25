namespace MealPlanner.Dto
{
    public class RecipeNutritionUpdateDto
    {
        public int Id { get; set; }
        public int RecipeId { get; set; }  // Add this property
        public decimal TotalCalories { get; set; }
        public decimal TotalProteinG { get; set; }
        public decimal TotalCarbsG { get; set; }
        public decimal TotalFatG { get; set; }
        public decimal TotalFiberG { get; set; }
        public decimal TotalSugarG { get; set; }
        public decimal TotalSodiumG { get; set; }
        public decimal CaloriesPerServing { get; set; }
    }
}