using Data.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ViaEurope.Business.Helpers;
using ViaEurope.Business.Services;
using ViaEurope.Data;
using ViaEurope.Data.Repositories.Implementations;

namespace ViaEurope.WPF.Services
{
    public class ServiceLocator
    {
        private static ServiceLocator? _instance;
        public static ServiceLocator Instance => _instance ??= new ServiceLocator();

        public AuthService AuthService { get; }
        public MenuService MenuService { get; }
        public OrderService OrderService { get; }
        public NavigationService NavigationService { get; }

        private ServiceLocator()
        {
            // Config
            var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            AppConfig.Initialize(config);

            var connectionString = config.GetConnectionString("ViaEuropeDB")!;

            // DbContext factory
            ViaEuropeContext CreateContext()
            {
                var options = new DbContextOptionsBuilder<ViaEuropeContext>()
                    .UseSqlServer(connectionString)
                    .Options;
                return new ViaEuropeContext(options);
            }

            // Repositories
            var userRepo = new UserRepository(CreateContext());
            var categoryRepo = new CategoryRepository(CreateContext());
            var dishRepo = new DishRepository(CreateContext());
            var menuRepo = new MenuRepository(CreateContext());
            var orderRepo = new OrderRepository(CreateContext());

            // Services
            AuthService = new AuthService(userRepo);
            MenuService = new MenuService(categoryRepo, dishRepo, menuRepo);
            OrderService = new OrderService(orderRepo, dishRepo);
            NavigationService = new NavigationService();
        }
    }
}