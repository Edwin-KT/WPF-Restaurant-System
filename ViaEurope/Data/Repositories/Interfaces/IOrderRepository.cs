using ViaEurope.Data.Models;

namespace ViaEurope.Data.Repositories.Interfaces
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<IEnumerable<Order>> GetAllWithDetailsAsync();
        Task<IEnumerable<Order>> GetByUserIdAsync(int userId);
        Task<IEnumerable<Order>> GetActiveOrdersAsync();
        Task<Order?> GetWithDetailsByIdAsync(int orderId);
        Task<int> CountRecentOrdersByUserAsync(int userId, int days);
        Task UpdateOrderStatusSpAsync(int orderId, string newStatus);
    }
}