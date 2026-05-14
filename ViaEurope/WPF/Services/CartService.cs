using System.Collections.ObjectModel;
using ViaEurope.Data.Models;

namespace ViaEurope.WPF.Services
{
    public class CartItem
    {
        public Dish? Dish { get; set; }
        public Menu? Menu { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string Name => Dish?.Name ?? Menu?.Name ?? string.Empty;
        public decimal Total => Quantity * UnitPrice;
    }

    public class CartService
    {
        private static CartService? _instance;
        public static CartService Instance => _instance ??= new CartService();

        public ObservableCollection<CartItem> Items { get; } = new();

        public void AddDish(Dish dish)
        {
            var existing = Items.FirstOrDefault(i => i.Dish?.DishId == dish.DishId);
            if (existing != null)
                existing.Quantity++;
            else
                Items.Add(new CartItem
                {
                    Dish = dish,
                    Quantity = 1,
                    UnitPrice = dish.Price
                });
        }

        public void AddMenu(Menu menu, decimal price)
        {
            var existing = Items.FirstOrDefault(i => i.Menu?.MenuId == menu.MenuId);
            if (existing != null)
                existing.Quantity++;
            else
                Items.Add(new CartItem
                {
                    Menu = menu,
                    Quantity = 1,
                    UnitPrice = price
                });
        }

        public void Remove(CartItem item) => Items.Remove(item);

        public void Clear() => Items.Clear();

        public decimal Total => Items.Sum(i => i.Total);
    }
}