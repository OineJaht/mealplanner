namespace MealPlanner.Dto
{
    public class ShoppingListDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<ShoppingListItemDto> Items { get; set; } = new ();
        
    }
}