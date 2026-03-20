using Microsoft.EntityFrameworkCore;
using EBikeAPI.Models;

namespace EBikeAPI.Data;

public class AppDbContext : DbContext
{
    // Constructor bắt buộc để nhận Connection String từ Program.cs
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Inventory> Inventories { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Warranty> Warranties { get; set; }
}