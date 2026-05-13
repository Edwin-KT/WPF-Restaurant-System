namespace ViaEurope.Data.Models
{
    public class DishPhoto
    {
        public int DishPhotoId { get; set; }
        public string PhotoPath { get; set; } = null!;

        public int DishId { get; set; }
        public Dish Dish { get; set; } = null!;
    }
}