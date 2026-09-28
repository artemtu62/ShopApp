using ShopApp.Common.Models;
using ShopApp.DAL.EFContext;

namespace ShopApp.DAL.Repositories;

public class OrderRepository
{
    private readonly ShopDbContext _ctx;

    public OrderRepository(ShopDbContext ctx) => _ctx = ctx;

    public IEnumerable<Order> GetAll() => _ctx.Orders.ToList();
}