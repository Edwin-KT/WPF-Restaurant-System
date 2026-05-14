using ViaEurope.Business.Helpers;
using ViaEurope.Data.Models;
using ViaEurope.Data.Repositories.Interfaces;

namespace ViaEurope.Business.Services
{
    public class MenuService
    {
        private readonly ICategoryRepository _categoryRepo;
        private readonly IDishRepository _dishRepo;
        private readonly IMenuRepository _menuRepo;

        public MenuService(ICategoryRepository categoryRepo,
                           IDishRepository dishRepo,
                           IMenuRepository menuRepo)
        {
            _categoryRepo = categoryRepo;
            _dishRepo = dishRepo;
            _menuRepo = menuRepo;
        }

        public async Task<IEnumerable<Category>> GetFullMenuAsync()
            => await _categoryRepo.GetCategoriesWithDishesAndMenusAsync();

        public async Task<IEnumerable<Dish>> SearchByKeywordAsync(string keyword, bool mustContain)
            => await _dishRepo.SearchByKeywordAsync(keyword, mustContain);

        public async Task<IEnumerable<Dish>> SearchByAllergenAsync(string allergen, bool mustContain)
            => await _dishRepo.SearchByAllergenAsync(allergen, mustContain);

        public async Task<IEnumerable<Dish>> GetLowStockAsync()
            => await _dishRepo.GetLowStockAsync(AppConfig.LowStockThreshold);

        // Pretul unui meniu = suma preparate - discount%
        public decimal CalculateMenuPrice(Menu menu)
        {
            var total = menu.MenuDishes.Sum(md => md.Dish.Price);
            var discount = total * AppConfig.MenuDiscountPercent / 100;
            return total - discount;
        }

        public bool IsMenuAvailable(Menu menu)
            => menu.MenuDishes.All(md => md.Dish.TotalQuantity > 0);

        // CRUD Categorii
        public async Task AddCategoryAsync(Category category)
            => await _categoryRepo.AddAsync(category);

        public async Task UpdateCategoryAsync(Category category)
            => await _categoryRepo.UpdateAsync(category);

        public async Task DeleteCategoryAsync(int id)
            => await _categoryRepo.DeleteAsync(id);

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
            => await _categoryRepo.GetAllAsync();

        // CRUD Preparate
        public async Task AddDishAsync(Dish dish)
            => await _dishRepo.AddAsync(dish);

        public async Task UpdateDishAsync(Dish dish)
            => await _dishRepo.UpdateAsync(dish);

        public async Task DeleteDishAsync(int id)
            => await _dishRepo.DeleteAsync(id);

        public async Task<IEnumerable<Dish>> GetAllDishesAsync()
            => await _dishRepo.GetAllWithDetailsAsync();

        // CRUD Meniuri
        public async Task AddMenuAsync(Menu menu)
            => await _menuRepo.AddAsync(menu);

        public async Task UpdateMenuAsync(Menu menu)
            => await _menuRepo.UpdateAsync(menu);

        public async Task DeleteMenuAsync(int id)
            => await _menuRepo.DeleteAsync(id);

        public async Task<IEnumerable<Menu>> GetAllMenusAsync()
            => await _menuRepo.GetAllWithDetailsAsync();
    }
}