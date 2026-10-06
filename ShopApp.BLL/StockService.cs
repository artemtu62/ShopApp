using ShopApp.Common.Models;
 
namespace ShopApp.BLL;
 
public class StockService
{
    private readonly IProductStockRepository _repository;
 
    public StockService(IProductStockRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }
 
    public void ReserveStock(int productId, int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Количество должно быть положительным.");
        }
 
        Product? product = _repository.GetById(productId);
        if (product is null)
        {
            throw new ProductNotFoundException(productId);
        }
 
        if (product.StockQuantity < quantity)
        {
            throw new InsufficientStockException(productId, quantity, product.StockQuantity);
        }
 
        product.StockQuantity -= quantity;
        _repository.Update(product);
    }
 
    public void ReleaseStock(int productId, int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Количество должно быть положительным.");
        }
 
        Product? product = _repository.GetById(productId);
        if (product is null)
        {
            throw new ProductNotFoundException(productId);
        }
 
        product.StockQuantity += quantity;
        _repository.Update(product);
    }
 
    public bool HasEnoughStock(int productId, int quantity)
    {
        Product? product = _repository.GetById(productId);
        if (product is null)
        {
            throw new ProductNotFoundException(productId);
        }
 
        return product.StockQuantity >= quantity;
    }
}
