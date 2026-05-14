using ViaEurope.Data.Models;

namespace ViaEurope.Data.Repositories.Interfaces
{
    public interface IDishRepository : IGenericRepository<Dish>
    {
        Task<IEnumerable<Dish>> GetAllWithDetailsAsync();
        Task<IEnumerable<Dish>> SearchByKeywordAsync(string keyword, bool mustContain);
        Task<IEnumerable<Dish>> SearchByAllergenAsync(string allergen, bool mustContain);
        Task<IEnumerable<Dish>> GetLowStockAsync(decimal threshold);
        Task UpdateQuantityAfterDeliveryAsync(int orderId);
    }
}