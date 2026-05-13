using Microsoft.EntityFrameworkCore;
using ViaEurope.Data.Models;

namespace ViaEurope.Data
{
    public class ViaEuropeContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Allergen> Allergens { get; set; }
        public DbSet<Dish> Dishes { get; set; }
        public DbSet<DishPhoto> DishPhotos { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<MenuDish> MenuDishes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        public ViaEuropaContext(DbContextOptions<ViaEuropaContext> options)
            : base(options) { }

        public ViaEuropaContext() { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    "Server=.;Database=ViaEuropaDB;Trusted_Connection=True;TrustServerCertificate=True;"
                );
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(e =>
            {
                e.HasKey(c => c.CategoryId);
                e.Property(c => c.Name).IsRequired().HasMaxLength(100);
                e.Property(c => c.Description).HasMaxLength(500);
            });

            modelBuilder.Entity<Allergen>(e =>
            {
                e.HasKey(a => a.AllergenId);
                e.Property(a => a.Name).IsRequired().HasMaxLength(100);
            });

            modelBuilder.Entity<Dish>(e =>
            {
                e.HasKey(d => d.DishId);
                e.Property(d => d.Name).IsRequired().HasMaxLength(150);
                e.Property(d => d.Price).HasPrecision(10, 2);
                e.Property(d => d.PortionQuantity).HasPrecision(10, 2);
                e.Property(d => d.TotalQuantity).HasPrecision(10, 2);
                e.Property(d => d.PortionUnit).HasMaxLength(20);
                e.Property(d => d.TotalUnit).HasMaxLength(20);
                e.Property(d => d.CountryOfOrigin).HasMaxLength(100);

                e.HasOne(d => d.Category)
                 .WithMany(c => c.Dishes)
                 .HasForeignKey(d => d.CategoryId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasMany(d => d.Allergens)
                 .WithMany(a => a.Dishes)
                 .UsingEntity(j => j.ToTable("DishAllergens"));
            });

            modelBuilder.Entity<DishPhoto>(e =>
            {
                e.HasKey(p => p.DishPhotoId);
                e.Property(p => p.PhotoPath).IsRequired().HasMaxLength(500);

                e.HasOne(p => p.Dish)
                 .WithMany(d => d.Photos)
                 .HasForeignKey(p => p.DishId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Menu>(e =>
            {
                e.HasKey(m => m.MenuId);
                e.Property(m => m.Name).IsRequired().HasMaxLength(150);

                e.HasOne(m => m.Category)
                 .WithMany(c => c.Menus)
                 .HasForeignKey(m => m.CategoryId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<MenuDish>(e =>
            {
                e.HasKey(md => md.MenuDishId);
                e.Property(md => md.PortionQuantity).HasPrecision(10, 2);
                e.Property(md => md.PortionUnit).HasMaxLength(20);

                e.HasOne(md => md.Menu)
                 .WithMany(m => m.MenuDishes)
                 .HasForeignKey(md => md.MenuId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(md => md.Dish)
                 .WithMany(d => d.MenuDishes)
                 .HasForeignKey(md => md.DishId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<User>(e =>
            {
                e.HasKey(u => u.UserId);
                e.Property(u => u.Email).IsRequired().HasMaxLength(200);
                e.HasIndex(u => u.Email).IsUnique();
                e.Property(u => u.FirstName).IsRequired().HasMaxLength(100);
                e.Property(u => u.LastName).IsRequired().HasMaxLength(100);
                e.Property(u => u.PasswordHash).IsRequired().HasMaxLength(256);
                e.Property(u => u.Role).HasMaxLength(20).HasDefaultValue("Client");
            });

            modelBuilder.Entity<Order>(e =>
            {
                e.HasKey(o => o.OrderId);
                e.Property(o => o.OrderCode).IsRequired().HasMaxLength(50);
                e.HasIndex(o => o.OrderCode).IsUnique();
                e.Property(o => o.Status).HasMaxLength(50).HasDefaultValue("Inregistrata");
                e.Property(o => o.FoodCost).HasPrecision(10, 2);
                e.Property(o => o.TransportCost).HasPrecision(10, 2);
                e.Property(o => o.DiscountAmount).HasPrecision(10, 2);
                e.Property(o => o.TotalCost).HasPrecision(10, 2);

                e.HasOne(o => o.User)
                 .WithMany(u => u.Orders)
                 .HasForeignKey(o => o.UserId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<OrderItem>(e =>
            {
                e.HasKey(oi => oi.OrderItemId);
                e.Property(oi => oi.UnitPrice).HasPrecision(10, 2);

                e.HasOne(oi => oi.Order)
                 .WithMany(o => o.OrderItems)
                 .HasForeignKey(oi => oi.OrderId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(oi => oi.Dish)
                 .WithMany(d => d.OrderItems)
                 .HasForeignKey(oi => oi.DishId)
                 .IsRequired(false)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(oi => oi.Menu)
                 .WithMany(m => m.OrderItems)
                 .HasForeignKey(oi => oi.MenuId)
                 .IsRequired(false)
                 .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}