using System.Windows;
using System.Windows.Controls;
using ViaEurope.Data.Models;
using ViaEurope.WPF.Services;
using ViaEurope.WPF.ViewModels;

namespace ViaEurope.WPF.Views
{
    public partial class MenuPage : Page
    {
        private readonly MenuViewModel _vm;

        public MenuPage()
        {
            InitializeComponent();
            _vm = (MenuViewModel)DataContext;
            Loaded += async (s, e) => await _vm.LoadMenuAsync();
        }


        private void BtnAddToCart_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Dish dish)
            {
                CartService.Instance.AddDish(dish);
                if (Application.Current.MainWindow is MainWindow main)
                    main.UpdateCartCount();
                MessageBox.Show($"✅ {dish.Name} adăugat în coș!",
                "Coș", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void BtnAddMenuToCart_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Data.Models.Menu menu)
            {
                var price = ServiceLocator.Instance.MenuService.CalculateMenuPrice(menu);
                CartService.Instance.AddMenu(menu, price);
                if (Application.Current.MainWindow is MainWindow main)
                    main.UpdateCartCount();
                MessageBox.Show($"✅ {menu.Name} adăugat în coș!\nPreț: {price:0.00} lei",
                    "Coș", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void TxtMenuPrice_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is TextBlock tb && tb.Tag is Data.Models.Menu menu)
            {
                var price = ServiceLocator.Instance.MenuService.CalculateMenuPrice(menu);
                tb.Text = $"{price:0.00} lei";
            }
        }
    }
}