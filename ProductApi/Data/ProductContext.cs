using Microsoft.EntityFrameworkCore;
using ProductApi.Models;

namespace ProductApi.Data
{
    public class ProductContext : DbContext
    {
        public ProductContext(DbContextOptions<ProductContext> options) : base(options) { }
      

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Laptop", Brand = "Dell", Price = 12999, Stock = 50 },
                new Product { Id = 2, Name = "Smartphone", Brand = "Samsung", Price = 7999, Stock = 100 },
                new Product { Id = 3, Name = "Tablet", Brand = "Apple", Price = 9999, Stock = 30 },
                new Product { Id = 4, Name = "Smartwatch", Brand = "Fitbit", Price = 2499, Stock = 80 }
            );
        }
    }
}
