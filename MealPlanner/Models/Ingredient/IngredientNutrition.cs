using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MealPlanner.Models.Inventory
{
    public class IngredientNutrition
    {
        [Key]
        public int Id { get; set; }
        
        public int IngredientId { get; set; }
        
        public decimal CaloriesPer100g { get; set; }
        
        public decimal ProteinG { get; set; }
        
        public decimal CarbsG { get; set; }
        
        public decimal FatG { get; set; }
        
        public decimal FiberG { get; set; }
        
        public decimal SugarG { get; set; }
        
        public decimal SodiumMg { get; set; }

        // Navigation property
        [ForeignKey("IngredientId")] 
        public virtual Ingredient Ingredient { get; set; } = null!; // Use null! to indicate it will be set after construction
    }
}
