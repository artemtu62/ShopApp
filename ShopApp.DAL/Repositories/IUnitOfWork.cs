using ShopApp.Common.Models;

namespace ShopApp.DAL.Repositories;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<Client> Clients { get; }
    IGenericRepository<Product> Products { get; }
    IGenericRepository<Order> Orders { get; }
    IGenericRepository<OrderItem> OrderItems { get; }

    int Complete();
    Task<int> CompleteAsync();
}