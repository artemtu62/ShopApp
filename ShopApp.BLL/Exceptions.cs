namespace ShopApp.BLL;
 
public class ProductNotFoundException : Exception
{
    public int ProductId { get; }
 
    public ProductNotFoundException(int productId)
        : base($"Товар с Id={productId} не найден.")
    {
        ProductId = productId;
    }
}
 
public class InsufficientStockException : Exception
{
    public int ProductId { get; }
    public int Requested { get; }
    public int Available { get; }
 
    public InsufficientStockException(int productId, int requested, int available)
        : base($"Недостаточно товара Id={productId} на складе: запрошено {requested}, доступно {available}.")
    {
        ProductId = productId;
        Requested = requested;
        Available = available;
    }
}
 
public class InvalidCouponException : Exception
{
    public string CouponCode { get; }
 
    public InvalidCouponException(string couponCode, string reason)
        : base($"Промокод \"{couponCode}\" недействителен: {reason}")
    {
        CouponCode = couponCode;
    }
}
