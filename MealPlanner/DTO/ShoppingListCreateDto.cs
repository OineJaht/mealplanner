using System.ComponentModel.DataAnnotations;

namespace MealPlanner.Dto
{
    public class ShoppingListCreateDto
    {
        [Required]
        [StringLength(255)]
        public string? Name { get; set; }
    }
}