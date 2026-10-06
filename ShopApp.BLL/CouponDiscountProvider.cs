namespace ShopApp.BLL;

public class CouponDiscountProvider : IDiscountProvider
{
    private readonly Dictionary<string, decimal> _activeCoupons = new(StringComparer.OrdinalIgnoreCase)
    {
        ["WELCOME10"] = 10m,
        ["SALE25"] = 25m,
        ["VIP50"] = 50m,
    };
 
    public decimal GetDiscountPercent(string couponCode)
    {
        if (!_activeCoupons.TryGetValue(couponCode, out decimal percent))
        {
            throw new KeyNotFoundException($"Промокод \"{couponCode}\" не найден или не активен.");
        }
 
        return percent;
    }
}
