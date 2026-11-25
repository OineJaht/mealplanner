using System.ComponentModel.DataAnnotations;

namespace MealPlanner.Dto
{
    public class UserPantryItemCreateDto
    {
        [Required(ErrorMessage = "Please select an ingredient")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid ingredient")]
        public int IngredientId { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal AmountInStock { get; set; }
        
        [Required]
        [StringLength(50)]
        public string? Unit { get; set; }
        
        [StringLength(255)]
        public string Location { get; set; } = string.Empty;
        [Required]
        public DateTime ExpiryDate { get; set; }
    }
}