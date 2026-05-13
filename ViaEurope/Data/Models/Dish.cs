namespace ViaEuropa.Data.Models
{
    public class Dish
    {
        public int DishId { get; set; }
        public string Name { get; set; } = null!;
        public string? CountryOfOrigin { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }

        public decimal PortionQuantity { get; set; }
        public string PortionUnit { get; set; } = "g";

        public decimal TotalQuantity { get; set; }
        public string TotalUnit { get; set; } = "g";

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public ICollection<Allergen> Allergens { get; set; } = new List<Allergen>();
        public ICollection<DishPhoto> Photos { get; set; } = new List<DishPhoto>();
        public ICollection<MenuDish> MenuDishes { get; set; } = new List<MenuDish>();
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}