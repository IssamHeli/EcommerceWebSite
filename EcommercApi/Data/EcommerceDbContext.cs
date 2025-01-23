

using Microsoft.EntityFrameworkCore;

using EcommercApi.Models;

namespace EcommercApi.Data
{

    public class EcommerceDbContext : DbContext
    {
        public EcommerceDbContext(DbContextOptions<EcommerceDbContext> options) : base(options)
        {
           
        }

        // DbSet properties for each model
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure relationships and constraints
            // Product model configuration
            modelBuilder.Entity<Product>()
                .HasKey(p => p.Id);  // Ensure Id is the primary key
            modelBuilder.Entity<Product>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();
            // Order model configuration
            modelBuilder.Entity<Order>()
                .HasKey(o => o.Id);  // Ensure Id is the primary key
            modelBuilder.Entity<Order>()
                .Property(o => o.Id)
                .ValueGeneratedOnAdd();  // Ensure auto-increment

            // Admin model configuration
            modelBuilder.Entity<Admin>()
                .HasKey(a => a.Id);  // Ensure Id is the primary key
            modelBuilder.Entity<Admin>()
                .Property(a => a.Id)
                .ValueGeneratedOnAdd();  // Ensure auto-increment
        }
    }

}