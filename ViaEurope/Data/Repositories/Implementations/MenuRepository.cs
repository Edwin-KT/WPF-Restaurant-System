using Data.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using ViaEurope.Data.Models;
using ViaEurope.Data.Repositories.Interfaces;

namespace ViaEurope.Data.Repositories.Implementations
{
    public class MenuRepository : GenericRepository<Menu>, IMenuRepository
    {
        public MenuRepository(ViaEuropeContext context) : base(context) { }

        public async Task<IEnumerable<Menu>> GetAllWithDetailsAsync()
            => await _context.Menus
                .Include(m => m.Category)
                .Include(m => m.MenuDishes).ThenInclude(md => md.Dish)
                    .ThenInclude(d => d.Allergens)
                .ToListAsync();
    }
}