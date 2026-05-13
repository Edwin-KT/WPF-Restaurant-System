namespace ViaEurope.Data.Models
{
    public class MenuDish
    {
        public int MenuDishId { get; set; }
        public decimal PortionQuantity { get; set; }
        public string PortionUnit { get; set; } = "g";

        public int MenuId { get; set; }
        public Menu Menu { get; set; } = null!;

        public int DishId { get; set; }
        public Dish Dish { get; set; } = null!;
    }
}