using System.Collections.ObjectModel;
using System.Windows.Input;
using ViaEurope.Business.Services;
using ViaEurope.Data.Models;
using ViaEurope.WPF.Commands;
using ViaEurope.WPF.Services;

namespace ViaEurope.WPF.ViewModels
{
    public class OrdersViewModel : BaseViewModel
    {
        private readonly OrderService _orderService;
        private readonly AuthService _authService;

        private ObservableCollection<Order> _orders = new();
        private bool _isLoading;
        private string _errorMessage = string.Empty;
        private string _successMessage = string.Empty;

        public ObservableCollection<Order> Orders
        {
            get => _orders;
            set => SetProperty(ref _orders, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public string SuccessMessage
        {
            get => _successMessage;
            set => SetProperty(ref _successMessage, value);
        }

        public CartViewModel CartViewModel { get; } = new CartViewModel();

        public ICommand PlaceOrderCommand { get; }
        public ICommand CancelOrderCommand { get; }
        public ICommand LoadOrdersCommand { get; }

        public OrdersViewModel()
        {
            _orderService = ServiceLocator.Instance.OrderService;
            _authService = ServiceLocator.Instance.AuthService;

            PlaceOrderCommand = new RelayCommand(
                async _ => await PlaceOrderAsync(),
                _ => CartViewModel.HasItems && !IsLoading);

            CancelOrderCommand = new RelayCommand(
                async p => await CancelOrderAsync(p as Order),
                p => p is Order o &&
                     o.Status != "Livrata" &&
                     o.Status != "Anulata");

            LoadOrdersCommand = new RelayCommand(
                async _ => await LoadOrdersAsync());
        }

        public async Task LoadOrdersAsync()
        {
            if (AuthService.CurrentUser == null) return;

            IsLoading = true;
            ErrorMessage = string.Empty;
            try
            {
                var orders = await _orderService.GetUserOrdersAsync(
                    AuthService.CurrentUser.UserId);
                Orders = new ObservableCollection<Order>(orders);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Eroare: {ex.Message}";
            }
            finally { IsLoading = false; }
        }

        private async Task PlaceOrderAsync()
        {
            if (AuthService.CurrentUser == null) return;

            IsLoading = true;
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;

            try
            {
                var items = CartViewModel.Items.Select(ci => new Data.Models.OrderItem
                {
                    DishId = ci.Dish?.DishId,
                    MenuId = ci.Menu?.MenuId,
                    Quantity = ci.Quantity,
                    UnitPrice = ci.UnitPrice
                }).ToList();

                var order = await _orderService.PlaceOrderAsync(
                    AuthService.CurrentUser.UserId, items);

                CartService.Instance.Clear();
                CartViewModel.Refresh();

                SuccessMessage = $"✅ Comanda {order.OrderCode} a fost plasată! " +
                                 $"Livrare estimată: {order.EstimatedDelivery:HH:mm}";

                await LoadOrdersAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Eroare la plasare comandă: {ex.Message}";
            }
            finally { IsLoading = false; }
        }

        private async Task CancelOrderAsync(Order? order)
        {
            if (order == null) return;

            try
            {
                await _orderService.CancelOrderAsync(order.OrderId);
                SuccessMessage = $"Comanda {order.OrderCode} a fost anulată.";
                await LoadOrdersAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Eroare: {ex.Message}";
            }
        }
    }
}