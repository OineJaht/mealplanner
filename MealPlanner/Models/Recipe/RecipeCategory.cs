using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MealPlanner.Models.Inventory;


namespace MealPlanner.Models.Inventory
{
    public class RecipeCategory
    {
        [Key]
        public int Id { get; set; }
        
        public int RecipeId { get; set; }
        
        public int CategoryId { get; set; }

        // Navigation properties
        [ForeignKey("RecipeId")] public virtual Recipe Recipe { get; set; } = null!;
        
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; } = null!;
    }
}
