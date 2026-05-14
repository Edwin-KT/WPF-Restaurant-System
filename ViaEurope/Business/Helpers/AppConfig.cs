using Microsoft.Extensions.Configuration;

namespace ViaEurope.Business.Helpers
{
    public static class AppConfig
    {
        private static IConfiguration? _config;

        public static void Initialize(IConfiguration config)
        {
            _config = config;
        }

        public static decimal MenuDiscountPercent =>
            decimal.Parse(_config!["RestaurantConfig:MenuDiscountPercent"]!);

        public static decimal OrderDiscountPercent =>
            decimal.Parse(_config!["RestaurantConfig:OrderDiscountPercent"]!);

        public static decimal OrderDiscountThreshold =>
            decimal.Parse(_config!["RestaurantConfig:OrderDiscountThreshold"]!);

        public static int LoyaltyOrderCount =>
            int.Parse(_config!["RestaurantConfig:LoyaltyOrderCount"]!);

        public static int LoyaltyIntervalDays =>
            int.Parse(_config!["RestaurantConfig:LoyaltyIntervalDays"]!);

        public static decimal FreeDeliveryThreshold =>
            decimal.Parse(_config!["RestaurantConfig:FreeDeliveryThreshold"]!);

        public static decimal DeliveryCost =>
            decimal.Parse(_config!["RestaurantConfig:DeliveryCost"]!);

        public static decimal LowStockThreshold =>
            decimal.Parse(_config!["RestaurantConfig:LowStockThreshold"]!);
    }
}