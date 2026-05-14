using Microsoft.EntityFrameworkCore;
using ViaEurope.Data.Models;
using ViaEurope.Data.Repositories.Interfaces;

namespace ViaEurope.Data.Repositories.Implementations
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(ViaEuropeContext context) : base(context) { }

        public async Task<IEnumerable<Category>> GetCategoriesWithDishesAndMenusAsync()
            => await _context.Categories
                .Include(c => c.Dishes).ThenInclude(d => d.Photos)
                .Include(c => c.Dishes).ThenInclude(d => d.Allergens)
                .Include(c => c.Menus).ThenInclude(m => m.MenuDishes).ThenInclude(md => md.Dish)
                .ToListAsync();
    }
}