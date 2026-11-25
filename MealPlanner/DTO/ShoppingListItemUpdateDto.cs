namespace MealPlanner.Dto
{
    public class ShoppingListItemUpdateDto
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public bool Purchased { get; set; }
    }
}