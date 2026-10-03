using ShopApp.Common.Models;

namespace ShopApp.DAL.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ShopDbContext _context;

    public UnitOfWork(ShopDbContext context)
    {
        _context = context;
        Clients = new GenericRepository<Client>(context);
        Products = new GenericRepository<Product>(context);
        Orders = new GenericRepository<Order>(context);
        OrderItems = new GenericRepository<OrderItem>(context);
    }

    public IGenericRepository<Client> Clients { get; }
    public IGenericRepository<Product> Products { get; }
    public IGenericRepository<Order> Orders { get; }
    public IGenericRepository<OrderItem> OrderItems { get; }

    public int Complete() => _context.SaveChanges();
    public Task<int> CompleteAsync() => _context.SaveChangesAsync();

    public void Dispose() => _context.Dispose();
}