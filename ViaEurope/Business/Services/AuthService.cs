using ViaEurope.Business.Helpers;
using ViaEurope.Data.Models;
using ViaEurope.Data.Repositories.Interfaces;

namespace ViaEurope.Business.Services
{
    public class AuthService
    {
        private readonly IUserRepository _userRepo;

        public static User? CurrentUser { get; private set; }

        public AuthService(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            var user = await _userRepo.GetByEmailAsync(email);
            if (user == null) return false;
            if (!PasswordHelper.Verify(password, user.PasswordHash)) return false;

            CurrentUser = user;
            return true;
        }

        public void Logout() => CurrentUser = null;

        public async Task<(bool Success, string Error)> RegisterAsync(
            string firstName, string lastName,
            string email, string phone,
            string deliveryAddress, string password)
        {
            if (await _userRepo.EmailExistsAsync(email))
                return (false, "Acest email este deja folosit.");

            var user = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Phone = phone,
                DeliveryAddress = deliveryAddress,
                PasswordHash = PasswordHelper.Hash(password),
                Role = "Client"
            };

            await _userRepo.InsertUserSpAsync(user);
            return (true, string.Empty);
        }

        public bool IsAuthenticated => CurrentUser != null;
        public bool IsClient => CurrentUser?.Role == "Client";
        public bool IsEmployee => CurrentUser?.Role == "Employee";
    }
}