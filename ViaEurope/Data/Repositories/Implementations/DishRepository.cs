using Data.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        // ---- APEL PROCEDURI SELECT ----
        public async Task<IEnumerable<Dish>> SearchByKeywordAsync(string keyword, bool mustContain)
        {
            return await _context.Dishes
                .FromSqlRaw("EXEC sp_SearchDishesByKeyword @Keyword = {0}, @MustContain = {1}", keyword, mustContain)
                .Include(d => d.Category)
                .Include(d => d.Allergens)
                .Include(d => d.Photos)
                .ToListAsync();
        }

        public async Task<IEnumerable<Dish>> SearchByAllergenAsync(string allergen, bool mustContain)
        {
            return await _context.Dishes
                .FromSqlRaw("EXEC sp_SearchDishesByAllergen @AllergenName = {0}, @MustContain = {1}", allergen, mustContain)
                .Include(d => d.Category)
                .Include(d => d.Allergens)
                .Include(d => d.Photos)
                .ToListAsync();
        }

        public async Task<IEnumerable<Dish>> GetLowStockAsync(decimal threshold)
        {
            return await _context.Dishes
                .FromSqlRaw("EXEC sp_GetLowStockDishes @Threshold = {0}", threshold)
                .ToListAsync();
        }

        public async Task UpdateQuantityAfterDeliveryAsync(int orderId)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC sp_UpdateDishQuantityAfterDelivery @OrderId = {0}", orderId);
        }
    }
}