using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MealPlanner.Models.Inventory;

namespace MealPlanner.Models.Inventory
{
    public class UserPantryItem
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(450)]
        public string UserId { get; set; } = string.Empty;
        
        public int IngredientId { get; set; }
        
        public decimal AmountInStock { get; set; }

        [Required] 
        [StringLength(50)] 
        public string Unit { get; set; } = string.Empty;
        
        [StringLength(255)]
        public string Location { get; set; } = string.Empty;
        
        public DateTime ExpiryDate { get; set; }

        // Navigation properties
        [ForeignKey("UserId")] 
        public virtual ApplicationUser? User { get; set; } = null!;
        
        [ForeignKey("IngredientId")]
        public virtual Ingredient? Ingredient { get; set; } = null!;
    }
}
