using System.Collections.ObjectModel;
using System.Windows.Input;
using ViaEurope.Business.Services;
using ViaEurope.Data.Models;
using ViaEurope.WPF.Commands;
using ViaEurope.WPF.Services;

namespace ViaEurope.WPF.ViewModels
{
    public class MenuViewModel : BaseViewModel
    {
        private readonly MenuService _menuService;

        private ObservableCollection<Category> _categories = new();
        private string _searchText = string.Empty;
        private bool _searchByAllergen;
        private bool _mustContain = true;
        private bool _isLoading;
        private string _errorMessage = string.Empty;

        public ObservableCollection<Category> Categories
        {
            get => _categories;
            set => SetProperty(ref _categories, value);
        }

        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        public bool SearchByAllergen
        {
            get => _searchByAllergen;
            set => SetProperty(ref _searchByAllergen, value);
        }

        public bool MustContain
        {
            get => _mustContain;
            set => SetProperty(ref _mustContain, value);
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

        public bool IsClient => ServiceLocator.Instance.AuthService.IsClient;

        public ICommand LoadMenuCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand ClearSearchCommand { get; }

        public MenuViewModel()
        {
            _menuService = ServiceLocator.Instance.MenuService;
            LoadMenuCommand = new RelayCommand(async _ => await LoadMenuAsync());
            SearchCommand = new RelayCommand(async _ => await SearchAsync());
            ClearSearchCommand = new RelayCommand(async _ => await LoadMenuAsync());
        }

        public async Task LoadMenuAsync()
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            try
            {
                var cats = await _menuService.GetFullMenuAsync();
                Categories = new ObservableCollection<Category>(cats);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Eroare la încărcarea meniului: {ex.Message}";
            }
            finally { IsLoading = false; }
        }

        private async Task SearchAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                await LoadMenuAsync();
                return;
            }

            IsLoading = true;
            try
            {
                IEnumerable<Dish> dishes;

                if (SearchByAllergen)
                    dishes = await _menuService.SearchByAllergenAsync(SearchText, MustContain);
                else
                    dishes = await _menuService.SearchByKeywordAsync(SearchText, MustContain);

                var grouped = dishes
                    .GroupBy(d => d.Category)
                    .Select(g =>
                    {
                        var cat = new Category
                        {
                            CategoryId = g.Key.CategoryId,
                            Name = g.Key.Name,
                            Description = g.Key.Description,
                            Dishes = g.ToList()
                        };
                        return cat;
                    });

                Categories = new ObservableCollection<Category>(grouped);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Eroare la căutare: {ex.Message}";
            }
            finally { IsLoading = false; }
        }
    }
}