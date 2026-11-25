using System.ComponentModel.DataAnnotations;

namespace MealPlanner.Dto
{
    public class RecipeUpdateDto
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(255)]
        public string? Name { get; set; }
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        [Required]
        [StringLength(2000)]
        public string? Instructions { get; set; }
        
        [Range(1, 100)]
        public int DefaultServings { get; set; }
        
        [Range(0, 1440)]
        public int PrepTimeMinutes { get; set; }
        
        public bool IsFavorite { get; set; }
        
        public string? ImageUrl { get; set; }
        
        // Related items
        public List<int> CategoryIds { get; set; } = new List<int>();
        public List<RecipeIngredientUpdateDto> Ingredients { get; set; } = new List<RecipeIngredientUpdateDto>();
    }
}