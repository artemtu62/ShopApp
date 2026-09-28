using ShopApp.Common.Models;
using ShopApp.DAL.Repositories;

namespace ShopApp.BLL.Services;

public class OrderService
{
    private readonly OrderRepository _repo;

    public OrderService(OrderRepository repo) => _repo = repo;

    public IEnumerable<Order> GetOrders() => _repo.GetAll();
}