using System.Collections.ObjectModel;
using System.Windows.Input;
using ViaEurope.Business.Services;
using ViaEurope.Data.Models;
using ViaEurope.WPF.Commands;
using ViaEurope.WPF.Services;

namespace ViaEurope.WPF.ViewModels
{
    public class EmployeeViewModel : BaseViewModel
    {
        private readonly OrderService _orderService;
        private readonly MenuService _menuService;

        // --- Comenzi ---
        private ObservableCollection<Order> _allOrders = new();
        private ObservableCollection<Order> _activeOrders = new();
        private Order? _selectedOrder;
        private string _selectedStatus = string.Empty;

        // --- Stoc scazut ---
        private ObservableCollection<Dish> _lowStockDishes = new();

        // --- CRUD Categorii ---
        private ObservableCollection<Category> _categories = new();
        private Category? _selectedCategory;
        private string _newCategoryName = string.Empty;
        private string _newCategoryDesc = string.Empty;

        // --- CRUD Preparate ---
        private ObservableCollection<Dish> _dishes = new();
        private Dish? _selectedDish;
        private string _newDishName = string.Empty;
        private decimal _newDishPrice;
        private decimal _newDishPortion;
        private string _newDishPortionUnit = "g";
        private decimal _newDishTotal;
        private string _newDishCountry = string.Empty;
        private Category? _newDishCategory;

        private bool _isLoading;
        private string _message = string.Empty;
        private bool _messageIsError;

        #region Properties

        public ObservableCollection<Order> AllOrders
        {
            get => _allOrders;
            set => SetProperty(ref _allOrders, value);
        }

        public ObservableCollection<Order> ActiveOrders
        {
            get => _activeOrders;
            set => SetProperty(ref _activeOrders, value);
        }

        public Order? SelectedOrder
        {
            get => _selectedOrder;
            set => SetProperty(ref _selectedOrder, value);
        }

        public string SelectedStatus
        {
            get => _selectedStatus;
            set => SetProperty(ref _selectedStatus, value);
        }

        public ObservableCollection<Dish> LowStockDishes
        {
            get => _lowStockDishes;
            set => SetProperty(ref _lowStockDishes, value);
        }

        public ObservableCollection<Category> Categories
        {
            get => _categories;
            set => SetProperty(ref _categories, value);
        }

        public Category? SelectedCategory
        {
            get => _selectedCategory;
            set => SetProperty(ref _selectedCategory, value);
        }

        public string NewCategoryName
        {
            get => _newCategoryName;
            set => SetProperty(ref _newCategoryName, value);
        }

        public string NewCategoryDesc
        {
            get => _newCategoryDesc;
            set => SetProperty(ref _newCategoryDesc, value);
        }

        public ObservableCollection<Dish> Dishes
        {
            get => _dishes;
            set => SetProperty(ref _dishes, value);
        }

        public Dish? SelectedDish
        {
            get => _selectedDish;
            set => SetProperty(ref _selectedDish, value);
        }

        public string NewDishName
        {
            get => _newDishName;
            set => SetProperty(ref _newDishName, value);
        }

        public decimal NewDishPrice
        {
            get => _newDishPrice;
            set => SetProperty(ref _newDishPrice, value);
        }

        public decimal NewDishPortion
        {
            get => _newDishPortion;
            set => SetProperty(ref _newDishPortion, value);
        }

        public string NewDishPortionUnit
        {
            get => _newDishPortionUnit;
            set => SetProperty(ref _newDishPortionUnit, value);
        }

        public decimal NewDishTotal
        {
            get => _newDishTotal;
            set => SetProperty(ref _newDishTotal, value);
        }

        public string NewDishCountry
        {
            get => _newDishCountry;
            set => SetProperty(ref _newDishCountry, value);
        }

        public Category? NewDishCategory
        {
            get => _newDishCategory;
            set => SetProperty(ref _newDishCategory, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        public bool MessageIsError
        {
            get => _messageIsError;
            set => SetProperty(ref _messageIsError, value);
        }

        public List<string> StatusOptions { get; } = new()
        {
            "Inregistrata", "Se pregateste",
            "A plecat la client", "Livrata", "Anulata"
        };

        #endregion

        #region Commands

        public ICommand LoadAllCommand { get; }
        public ICommand UpdateOrderStatusCommand { get; }
        public ICommand AddCategoryCommand { get; }
        public ICommand DeleteCategoryCommand { get; }
        public ICommand AddDishCommand { get; }
        public ICommand DeleteDishCommand { get; }

        #endregion

        public EmployeeViewModel()
        {
            _orderService = ServiceLocator.Instance.OrderService;
            _menuService = ServiceLocator.Instance.MenuService;

            LoadAllCommand = new RelayCommand(
                async _ => await LoadAllAsync());

            UpdateOrderStatusCommand = new RelayCommand(
                async _ => await UpdateOrderStatusAsync(),
                _ => SelectedOrder != null && !string.IsNullOrEmpty(SelectedStatus));

            AddCategoryCommand = new RelayCommand(
                async _ => await AddCategoryAsync(),
                _ => !string.IsNullOrWhiteSpace(NewCategoryName));

            DeleteCategoryCommand = new RelayCommand(
                async _ => await DeleteCategoryAsync(),
                _ => SelectedCategory != null);

            AddDishCommand = new RelayCommand(
                async _ => await AddDishAsync(),
                _ => !string.IsNullOrWhiteSpace(NewDishName)
                     && NewDishPrice > 0 && NewDishCategory != null);

            DeleteDishCommand = new RelayCommand(
                async _ => await DeleteDishAsync(),
                _ => SelectedDish != null);
        }

        public async Task LoadAllAsync()
        {
            IsLoading = true;
            try
            {
                var all = await _orderService.GetAllOrdersAsync();
                AllOrders = new ObservableCollection<Order>(all);

                var active = await _orderService.GetActiveOrdersAsync();
                ActiveOrders = new ObservableCollection<Order>(active);

                var low = await _menuService.GetLowStockAsync();
                LowStockDishes = new ObservableCollection<Dish>(low);

                var cats = await _menuService.GetAllCategoriesAsync();
                Categories = new ObservableCollection<Category>(cats);

                var dishes = await _menuService.GetAllDishesAsync();
                Dishes = new ObservableCollection<Dish>(dishes);
            }
            catch (Exception ex)
            {
                ShowMessage($"Eroare: {ex.Message}", true);
            }
            finally { IsLoading = false; }
        }

        private async Task UpdateOrderStatusAsync()
        {
            try
            {
                await _orderService.UpdateStatusAsync(
                    SelectedOrder!.OrderId, SelectedStatus);
                ShowMessage("Status actualizat cu succes!", false);
                await LoadAllAsync();
            }
            catch (Exception ex)
            {
                ShowMessage($"Eroare: {ex.Message}", true);
            }
        }

        private async Task AddCategoryAsync()
        {
            try
            {
                await _menuService.AddCategoryAsync(new Category
                {
                    Name = NewCategoryName,
                    Description = NewCategoryDesc
                });
                NewCategoryName = string.Empty;
                NewCategoryDesc = string.Empty;
                ShowMessage("Categorie adăugată!", false);
                await LoadAllAsync();
            }
            catch (Exception ex)
            {
                ShowMessage($"Eroare: {ex.Message}", true);
            }
        }

        private async Task DeleteCategoryAsync()
        {
            try
            {
                await _menuService.DeleteCategoryAsync(SelectedCategory!.CategoryId);
                ShowMessage("Categorie ștearsă!", false);
                await LoadAllAsync();
            }
            catch (Exception ex)
            {
                ShowMessage($"Eroare: {ex.Message}", true);
            }
        }

        private async Task AddDishAsync()
        {
            try
            {
                await _menuService.AddDishAsync(new Dish
                {
                    Name = NewDishName,
                    Price = NewDishPrice,
                    PortionQuantity = NewDishPortion,
                    PortionUnit = NewDishPortionUnit,
                    TotalQuantity = NewDishTotal,
                    TotalUnit = NewDishPortionUnit,
                    CountryOfOrigin = NewDishCountry,
                    CategoryId = NewDishCategory!.CategoryId
                });
                NewDishName = string.Empty;
                NewDishPrice = 0;
                NewDishPortion = 0;
                NewDishTotal = 0;
                NewDishCountry = string.Empty;
                ShowMessage("Preparat adăugat!", false);
                await LoadAllAsync();
            }
            catch (Exception ex)
            {
                ShowMessage($"Eroare: {ex.Message}", true);
            }
        }

        private async Task DeleteDishAsync()
        {
            try
            {
                await _menuService.DeleteDishAsync(SelectedDish!.DishId);
                ShowMessage("Preparat șters!", false);
                await LoadAllAsync();
            }
            catch (Exception ex)
            {
                ShowMessage($"Eroare: {ex.Message}", true);
            }
        }

        private void ShowMessage(string msg, bool isError)
        {
            Message = msg;
            MessageIsError = isError;
        }
    }
}