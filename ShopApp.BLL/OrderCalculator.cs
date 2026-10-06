using ShopApp.Common.Models;
 
namespace ShopApp.BLL;

public class OrderCalculator
{
    private readonly IDiscountProvider _discountProvider;
 
    public OrderCalculator(IDiscountProvider discountProvider)
    {
        _discountProvider = discountProvider ?? throw new ArgumentNullException(nameof(discountProvider));
    }

    public decimal CalculateSubtotal(Order order)
    {
        if (order is null)
        {
            throw new ArgumentNullException(nameof(order));
        }
 
        if (order.Items is null || order.Items.Count == 0)
        {
            throw new ArgumentException("Заказ должен содержать хотя бы одну позицию.", nameof(order));
        }
 
        decimal subtotal = 0m;
        foreach (OrderItem item in order.Items)
        {
            if (item.Quantity <= 0)
            {
                throw new ArgumentException($"Количество товара {item.ProductId} должно быть положительным.");
            }
 
            subtotal += item.Price * item.Quantity;
        }
 
        return subtotal;
    }

    public decimal CalculateTotal(Order order)
    {
        decimal subtotal = CalculateSubtotal(order);
        decimal percentOff = 0m;
 
        if (!string.IsNullOrWhiteSpace(order.CouponCode))
        {
            try
            {
                percentOff = _discountProvider.GetDiscountPercent(order.CouponCode);
            }
            catch (KeyNotFoundException ex)
            {
                // Оборачиваем низкоуровневое исключение провайдера в доменное —
                // вызывающий код BLL не должен знать о деталях реализации провайдера.
                throw new InvalidCouponException(order.CouponCode, ex.Message);
            }
 
            if (percentOff is < 0 or > 100)
            {
                throw new InvalidCouponException(order.CouponCode, "процент скидки вне диапазона 0–100.");
            }
        }
        decimal tierBonus = subtotal switch
        {
            >= 100000m => 7m,
            >= 50000m => 5m,
            >= 10000m => 2m,
            _ => 0m,
        };
 
        decimal totalPercentOff = Math.Min(percentOff + tierBonus, 100m);
        decimal total = subtotal * (1 - totalPercentOff / 100m);
 
        return Math.Round(total, 2, MidpointRounding.AwayFromZero);
    }
}
