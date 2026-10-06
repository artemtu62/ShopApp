namespace ShopApp.BLL;

public interface IDiscountProvider
{
    decimal GetDiscountPercent(string couponCode);
}