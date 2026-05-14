using Data.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using ViaEurope.Data.Models;
using ViaEurope.Data.Repositories.Interfaces;

namespace ViaEurope.Data.Repositories.Implementations
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(ViaEuropeContext context) : base(context) { }

        public async Task<IEnumerable<Order>> GetAllWithDetailsAsync()
            => await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Dish)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Menu)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

        public async Task<IEnumerable<Order>> GetByUserIdAsync(int userId)
            => await _context.Orders
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Dish)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Menu)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

        public async Task<IEnumerable<Order>> GetActiveOrdersAsync()
            => await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Dish)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Menu)
                .Where(o => o.Status != "Livrata" && o.Status != "Anulata")
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

        public async Task<Order?> GetWithDetailsByIdAsync(int orderId)
            => await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Dish)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Menu)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

        public async Task<int> CountRecentOrdersByUserAsync(int userId, int days)
            => await _context.Orders
                .Where(o => o.UserId == userId
                    && o.Status == "Livrata"
                    && o.OrderDate >= DateTime.Now.AddDays(-days))
                .CountAsync();
    }
}