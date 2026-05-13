namespace ViaEuropa.Data.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public string? DeliveryAddress { get; set; }
        public string PasswordHash { get; set; } = null!;

        // "Client" sau "Employee"
        public string Role { get; set; } = "Client";

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}