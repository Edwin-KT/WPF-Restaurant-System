using ViaEurope.Data.Models;

namespace ViaEurope.Data.Repositories.Interfaces
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<IEnumerable<Category>> GetCategoriesWithDishesAndMenusAsync();
    }
}