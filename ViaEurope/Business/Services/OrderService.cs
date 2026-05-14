using ViaEurope.Business.Helpers;
using ViaEurope.Data.Models;
using ViaEurope.Data.Repositories.Interfaces;

namespace ViaEurope.Business.Services
{
    public class OrderService
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IDishRepository _dishRepo;

        public OrderService(IOrderRepository orderRepo, IDishRepository dishRepo)
        {
            _orderRepo = orderRepo;
            _dishRepo = dishRepo;
        }

        public async Task<Order> PlaceOrderAsync(int userId, List<OrderItem> items)
        {
            var foodCost = items.Sum(i => i.Quantity * i.UnitPrice);
            var discount = await CalculateDiscountAsync(userId, foodCost);
            var transport = (foodCost - discount) < AppConfig.FreeDeliveryThreshold
                ? AppConfig.DeliveryCost : 0;
            var total = foodCost - discount + transport;

            var order = new Order
            {
                UserId = userId,
                OrderCode = OrderCodeGenerator.Generate(),
                OrderDate = DateTime.Now,
                Status = "Inregistrata",
                FoodCost = foodCost,
                TransportCost = transport,
                DiscountAmount = discount,
                TotalCost = total,
                EstimatedDelivery = DateTime.Now.AddMinutes(45),
                OrderItems = items
            };

            await _orderRepo.AddAsync(order);
            return order;
        }

        private async Task<decimal> CalculateDiscountAsync(int userId, decimal foodCost)
        {
            // Discount daca comanda > threshold
            if (foodCost > AppConfig.OrderDiscountThreshold)
                return foodCost * AppConfig.OrderDiscountPercent / 100;

            // Discount loialitate
            var recentOrders = await _orderRepo.CountRecentOrdersByUserAsync(
                userId, AppConfig.LoyaltyIntervalDays);

            if (recentOrders >= AppConfig.LoyaltyOrderCount)
                return foodCost * AppConfig.OrderDiscountPercent / 100;

            return 0;
        }

        public async Task<IEnumerable<Order>> GetUserOrdersAsync(int userId)
            => await _orderRepo.GetByUserIdAsync(userId);

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
            => await _orderRepo.GetAllWithDetailsAsync();

        public async Task<IEnumerable<Order>> GetActiveOrdersAsync()
            => await _orderRepo.GetActiveOrdersAsync();

        public async Task UpdateStatusAsync(int orderId, string newStatus)
        {
            var order = await _orderRepo.GetByIdAsync(orderId);
            if (order == null) return;

            order.Status = newStatus;
            await _orderRepo.UpdateAsync(order);

            if (newStatus == "Livrata")
                await _dishRepo.UpdateQuantityAfterDeliveryAsync(orderId);
        }

        public async Task CancelOrderAsync(int orderId)
            => await UpdateStatusAsync(orderId, "Anulata");
    }
}