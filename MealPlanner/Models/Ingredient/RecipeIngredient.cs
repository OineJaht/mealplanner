using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MealPlanner.Models.Inventory
{
    public class RecipeIngredient
    {
        [Key]
        public int Id { get; set; }
        
        public int RecipeId { get; set; }
        
        public int IngredientId { get; set; }
        
        public decimal Amount { get; set; }
        
        [NotMapped] // This will not be stored in the database
        public bool IsAvailable { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Unit { get; set; } = String.Empty;

        // Navigation properties
        [ForeignKey("RecipeId")] public virtual Recipe Recipe { get; set; } = null!;
        
        [ForeignKey("IngredientId")]
        public virtual Ingredient Ingredient { get; set; } = null!;
    }
}
