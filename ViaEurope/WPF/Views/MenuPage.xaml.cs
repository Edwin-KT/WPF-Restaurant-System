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
    }
}