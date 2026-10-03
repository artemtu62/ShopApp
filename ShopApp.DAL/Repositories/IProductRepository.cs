using ShopApp.Common.Models;

namespace ShopApp.DAL.Repositories;

public interface IProductRepository : IRepository<Product>
{
    IEnumerable<Product> GetByCategoryName(string categoryName);
    IEnumerable<Product> GetByPriceRange(decimal min, decimal max);
}