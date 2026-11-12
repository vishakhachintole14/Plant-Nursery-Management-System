
using ASP.NET_PlantsOrder.Models;
using Microsoft.EntityFrameworkCore;

namespace ASP.NET_PlantsOrder.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Plants> Plants { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderItem> OrderItems { get; set; } = null!;
        public DbSet<Payment> Payments { get; set; } = null!;

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Plants>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<OrderItem>()
                .Property(i => i.UnitPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderItems)
                .WithOne(i => i.Order)
                .HasForeignKey(i => i.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Category>().HasData(
                new Category
                {
                    CategoryId = 1,
                    CategoryName = "Flowering Plants",
                    Description = "Beautiful flowering plants"
                },
                new Category
                {
                    CategoryId = 2,
                    CategoryName = "Indoor Plants",
                    Description = "Plants for indoor decoration"
                },
                new Category
                {
                    CategoryId = 3,
                    CategoryName = "Medicinal Plants",
                    Description = "Plants with medicinal uses"
                }
            );

            modelBuilder.Entity<Plants>().HasData(
                new Plants
                {
                    PlantId = 1,
                    PlantName = "Rose",
                    Price = 100m,
                    Quantity = 50,
                    CategoryId = 1,
                    ImageUrl = "/PLANT/rose.jpeg"
                },
                new Plants
                {
                    PlantId = 2,
                    PlantName = "Money Plant",
                    Price = 150m,
                    Quantity = 30,
                    CategoryId = 2,
                    ImageUrl = "/PLANT/monyplant.jpeg"
                },
                new Plants
                {
                    PlantId = 3,
                    PlantName = "Tulsi",
                    Price = 50m,
                    Quantity = 40,
                    CategoryId = 3,
                    ImageUrl = "/PLANT/tulsi.jpeg"
                }
            );
        }
    }
}
