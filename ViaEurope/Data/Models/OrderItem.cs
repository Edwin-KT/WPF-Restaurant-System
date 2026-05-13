namespace ViaEurope.Data.Models
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        // Fie DishId fie MenuId — nu ambele
        public int? DishId { get; set; }
        public Dish? Dish { get; set; }

        public int? MenuId { get; set; }
        public Menu? Menu { get; set; }
    }
}