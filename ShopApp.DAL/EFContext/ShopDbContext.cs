using Microsoft.EntityFrameworkCore;
using ShopApp.Common.Models;

namespace ShopApp.DAL.EFContext;

public class ShopDbContext : DbContext
{
    public ShopDbContext(DbContextOptions<ShopDbContext> options) 
        : base(options) { }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
}