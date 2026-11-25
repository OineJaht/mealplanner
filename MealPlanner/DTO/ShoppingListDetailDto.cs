namespace MealPlanner.Dto
{
    public class ShoppingListDetailDto : ShoppingListDto
    {
        public int TotalItems => Items.Count;
        public int PurchasedItems => Items.Count(i => i.Purchased);
    }
}