using System.ComponentModel.DataAnnotations;

namespace MealPlanner.Dto
{
    public class ShoppingListUpdateDto
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(255)]
        public string? Name { get; set; }
        
        public DateTime CreatedDate { get; set; }
    }
}