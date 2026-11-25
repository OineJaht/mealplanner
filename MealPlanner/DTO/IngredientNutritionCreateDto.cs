namespace MealPlanner.Dto
{
    public class IngredientNutritionCreateDto
    {
        public decimal CaloriesPer100g { get; set; } = 0;
        public decimal ProteinG { get; set; } = 0;
        public decimal CarbsG { get; set; } = 0;
        public decimal FatG { get; set; } = 0;
        public decimal FiberG { get; set; } = 0;
        public decimal SugarG { get; set; } = 0;
        public decimal SodiumMg { get; set; } = 0;
    }
}