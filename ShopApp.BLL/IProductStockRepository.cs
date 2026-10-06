namespace ShopApp.BLL;
 
using ShopApp.Common.Models;
 
public interface IProductStockRepository
{
    Product? GetById(int id);
    void Update(Product product);
}
