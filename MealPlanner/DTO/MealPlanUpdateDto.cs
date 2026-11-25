using System.ComponentModel.DataAnnotations;

namespace MealPlanner.Dto
{
    public class MealPlanUpdateDto
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(255)]
        public string? Name { get; set; }
        
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}