namespace ViaEuropa.Data.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public ICollection<Dish> Dishes { get; set; } = new List<Dish>();
        public ICollection<Menu> Menus { get; set; } = new List<Menu>();
    }
}