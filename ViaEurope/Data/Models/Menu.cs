namespace ViaEuropa.Data.Models
{
    public class Menu
    {
        public int MenuId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public ICollection<MenuDish> MenuDishes { get; set; } = new List<MenuDish>();
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}