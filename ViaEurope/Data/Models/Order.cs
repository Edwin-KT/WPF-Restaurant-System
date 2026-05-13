namespace ViaEuropa.Data.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public string OrderCode { get; set; } = null!;
        public DateTime OrderDate { get; set; } = DateTime.Now;

        // Inregistrata / Se pregateste / A plecat la client / Livrata / Anulata
        public string Status { get; set; } = "Inregistrata";

        public decimal FoodCost { get; set; }
        public decimal TransportCost { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalCost { get; set; }
        public DateTime? EstimatedDelivery { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}