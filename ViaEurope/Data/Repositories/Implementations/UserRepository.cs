using Data.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using ViaEurope.Data.Models;
using ViaEurope.Data.Repositories.Interfaces;

namespace ViaEurope.Data.Repositories.Implementations
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(ViaEuropeContext context) : base(context) { }

        public async Task<User?> GetByEmailAsync(string email)
            => await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        public async Task<bool> EmailExistsAsync(string email)
            => await _context.Users.AnyAsync(u => u.Email == email);
    }
}