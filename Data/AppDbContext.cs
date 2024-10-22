using Microsoft.EntityFrameworkCore;
using SalesOrderApp.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace SalesOrderApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Seeding default Customers
            modelBuilder.Entity<Customer>().HasData(
                new Customer { Id = 1, Name = "Profescipta" },
                new Customer { Id = 2, Name = "Titan" },
                new Customer { Id = 3, Name = "Dips" }
            );
        }
    }

}
