using Data.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using ViaEurope.Data.Models;
using ViaEurope.Data.Repositories.Interfaces;

namespace ViaEurope.Data.Repositories.Implementations
{
    public class DishRepository : GenericRepository<Dish>, IDishRepository
    {
        public DishRepository(ViaEuropeContext context) : base(context) { }

        public async Task<IEnumerable<Dish>> GetAllWithDetailsAsync()
            => await _context.Dishes
                .Include(d => d.Category)
                .Include(d => d.Allergens)
                .Include(d => d.Photos)
                .ToListAsync();

        public async Task<IEnumerable<Dish>> SearchByKeywordAsync(string keyword, bool mustContain)
            => mustContain
                ? await _context.Dishes
                    .Include(d => d.Category).Include(d => d.Allergens).Include(d => d.Photos)
                    .Where(d => d.Name.Contains(keyword))
                    .ToListAsync()
                : await _context.Dishes
                    .Include(d => d.Category).Include(d => d.Allergens).Include(d => d.Photos)
                    .Where(d => !d.Name.Contains(keyword))
                    .ToListAsync();

        public async Task<IEnumerable<Dish>> SearchByAllergenAsync(string allergen, bool mustContain)
            => mustContain
                ? await _context.Dishes
                    .Include(d => d.Category).Include(d => d.Allergens).Include(d => d.Photos)
                    .Where(d => d.Allergens.Any(a => a.Name.Contains(allergen)))
                    .ToListAsync()
                : await _context.Dishes
                    .Include(d => d.Category).Include(d => d.Allergens).Include(d => d.Photos)
                    .Where(d => !d.Allergens.Any(a => a.Name.Contains(allergen)))
                    .ToListAsync();

        public async Task<IEnumerable<Dish>> GetLowStockAsync(decimal threshold)
            => await _context.Dishes
                .Where(d => d.TotalQuantity <= threshold)
                .OrderBy(d => d.TotalQuantity)
                .ToListAsync();

        public async Task UpdateQuantityAfterDeliveryAsync(int orderId)
        {
            // Preparate comandate direct
            var directItems = await _context.OrderItems
                .Include(oi => oi.Dish)
                .Where(oi => oi.OrderId == orderId && oi.DishId != null)
                .ToListAsync();

            foreach (var item in directItems)
            {
                item.Dish!.TotalQuantity -= item.Quantity * item.Dish.PortionQuantity;
            }

            // Preparate din meniuri
            var menuItems = await _context.OrderItems
                .Include(oi => oi.Menu).ThenInclude(m => m!.MenuDishes).ThenInclude(md => md.Dish)
                .Where(oi => oi.OrderId == orderId && oi.MenuId != null)
                .ToListAsync();

            foreach (var item in menuItems)
            {
                foreach (var menuDish in item.Menu!.MenuDishes)
                {
                    menuDish.Dish.TotalQuantity -= item.Quantity * menuDish.PortionQuantity;
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}