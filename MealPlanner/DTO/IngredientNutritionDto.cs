namespace MealPlanner.Dto
{
    public class IngredientNutritionDto
    {
        public int IngredientId { get; set; }
        public int Id { get; set; }
        public decimal CaloriesPer100g { get; set; }
        public decimal ProteinG { get; set; }
        public decimal CarbsG { get; set; }
        public decimal FatG { get; set; }
        public decimal FiberG { get; set; }
        public decimal SugarG { get; set; }
        public decimal SodiumMg { get; set; }
    }
}