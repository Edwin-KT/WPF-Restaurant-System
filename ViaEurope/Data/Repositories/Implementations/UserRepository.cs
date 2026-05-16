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
        public async Task InsertUserSpAsync(User user)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC sp_InsertUser @FirstName={0}, @LastName={1}, @Email={2}, @Phone={3}, @DeliveryAddress={4}, @PasswordHash={5}, @Role={6}",
                user.FirstName, user.LastName, user.Email, user.Phone ?? (object)DBNull.Value,
                user.DeliveryAddress ?? (object)DBNull.Value, user.PasswordHash, user.Role);
        }
    }
}