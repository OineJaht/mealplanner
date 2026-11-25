using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MealPlanner.Models.Inventory
{
    public class RecipeNutrition
    {
        [Key]
        public int Id { get; set; }
        
        public int RecipeId { get; set; }
        
        public decimal TotalCalories { get; set; }
        
        public decimal TotalProteinG { get; set; }
        
        public decimal TotalCarbsG { get; set; }
        
        public decimal TotalFatG { get; set; }
        
        public decimal TotalFiberG { get; set; }
        
        public decimal TotalSugarG { get; set; }
        
        public decimal TotalSodiumG { get; set; }
        
        public decimal CaloriesPerServing { get; set; }

        // Navigation property
        [ForeignKey("RecipeId")]
        public virtual Recipe Recipe { get; set; } = null!;
    }
}

