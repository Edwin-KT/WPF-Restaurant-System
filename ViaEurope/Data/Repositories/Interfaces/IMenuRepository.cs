using ViaEurope.Data.Models;

namespace ViaEurope.Data.Repositories.Interfaces
{
    public interface IMenuRepository : IGenericRepository<Menu>
    {
        Task<IEnumerable<Menu>> GetAllWithDetailsAsync();
    }
}