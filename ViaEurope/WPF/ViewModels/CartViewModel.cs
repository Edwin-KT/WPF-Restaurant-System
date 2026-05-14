using System.Collections.ObjectModel;
using System.Windows.Input;
using ViaEurope.WPF.Commands;
using ViaEurope.WPF.Services;

namespace ViaEurope.WPF.ViewModels
{
    public class CartViewModel : BaseViewModel
    {
        private readonly CartService _cart = CartService.Instance;

        public ObservableCollection<CartItem> Items => _cart.Items;

        public decimal Total => _cart.Total;
        public decimal DeliveryCost => Total < Business.Helpers.AppConfig.FreeDeliveryThreshold
            ? Business.Helpers.AppConfig.DeliveryCost : 0;
        public decimal GrandTotal => Total + DeliveryCost;
        public bool HasItems => Items.Count > 0;

        public ICommand RemoveItemCommand { get; }
        public ICommand ClearCartCommand { get; }

        public Action? OnPlaceOrder { get; set; }

        public CartViewModel()
        {
            RemoveItemCommand = new RelayCommand(p =>
            {
                if (p is CartItem item)
                {
                    _cart.Remove(item);
                    Refresh();
                }
            });

            ClearCartCommand = new RelayCommand(_ =>
            {
                _cart.Clear();
                Refresh();
            });

            _cart.Items.CollectionChanged += (s, e) => Refresh();
        }

        public void Refresh()
        {
            OnPropertyChanged(nameof(Items));
            OnPropertyChanged(nameof(Total));
            OnPropertyChanged(nameof(DeliveryCost));
            OnPropertyChanged(nameof(GrandTotal));
            OnPropertyChanged(nameof(HasItems));
        }
    }
}