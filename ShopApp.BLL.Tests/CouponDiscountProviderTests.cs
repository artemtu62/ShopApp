using ShopApp.BLL;
using Xunit;

namespace ShopApp.BLL.Tests;

public class CouponDiscountProviderTests
{
    [Theory]
    [InlineData("WELCOME10", 10)]
    [InlineData("welcome10", 10)]
    [InlineData("SALE25", 25)]
    [InlineData("VIP50", 50)]
    public void GetDiscountPercent_KnownCoupon_ReturnsPercent(string code, decimal expected)
    {
        var provider = new CouponDiscountProvider();
        Assert.Equal(expected, provider.GetDiscountPercent(code));
    }

    [Fact]
    public void GetDiscountPercent_UnknownCoupon_ThrowsKeyNotFound()
    {
        var provider = new CouponDiscountProvider();
        Assert.Throws<KeyNotFoundException>(() => provider.GetDiscountPercent("NOPE"));
    }
}