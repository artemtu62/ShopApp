using Moq;
using ShopApp.BLL;
using ShopApp.Common.Models;
using Xunit;

namespace ShopApp.BLL.Tests;

public class OrderCalculatorTests
{
    private static Order MakeOrder(decimal price, int qty, string? coupon = null) => new()
    {
        Id = 1,
        ClientId = 1,
        CouponCode = coupon,
        Items = new List<OrderItem>
        {
            new() { ProductId = 1, Price = price, Quantity = qty }
        }
    };

    // --- Конструктор ---

    [Fact]
    public void Constructor_NullProvider_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new OrderCalculator(null!));
    }

    // --- CalculateSubtotal ---

    [Fact]
    public void CalculateSubtotal_NullOrder_Throws()
    {
        var calc = new OrderCalculator(Mock.Of<IDiscountProvider>());
        Assert.Throws<ArgumentNullException>(() => calc.CalculateSubtotal(null!));
    }

    [Fact]
    public void CalculateSubtotal_EmptyItems_Throws()
    {
        var calc = new OrderCalculator(Mock.Of<IDiscountProvider>());
        var order = new Order { Items = new List<OrderItem>() };
        Assert.Throws<ArgumentException>(() => calc.CalculateSubtotal(order));
    }

    [Fact]
    public void CalculateSubtotal_NonPositiveQuantity_Throws()
    {
        var calc = new OrderCalculator(Mock.Of<IDiscountProvider>());
        var order = MakeOrder(price: 100m, qty: 0);
        Assert.Throws<ArgumentException>(() => calc.CalculateSubtotal(order));
    }

    [Fact]
    public void CalculateSubtotal_ValidItems_ReturnsSum()
    {
        var calc = new OrderCalculator(Mock.Of<IDiscountProvider>());
        var order = MakeOrder(price: 150m, qty: 3);
        Assert.Equal(450m, calc.CalculateSubtotal(order));
    }

    // --- CalculateTotal: без промокода, разные пороги накопительной скидки ---

    [Theory]
    [InlineData(100, 1, 100)]         // < 10 000 -> 0% бонус
    [InlineData(1000, 15, 14700)]     // 15 000 -> 2% бонус: 15000 * 0.98
    [InlineData(1000, 60, 57000)]     // 60 000 -> 5% бонус: 60000 * 0.95
    [InlineData(1000, 110, 102300)]   // 110 000 -> 7% бонус: 110000 * 0.93
    public void CalculateTotal_NoCoupon_AppliesTierBonus(decimal price, int qty, decimal expected)
    {
        var calc = new OrderCalculator(Mock.Of<IDiscountProvider>());
        var order = MakeOrder(price, qty);
        Assert.Equal(expected, calc.CalculateTotal(order));
    }

    [Fact]
    public void CalculateTotal_ValidCoupon_AppliesDiscount()
    {
        var providerMock = new Mock<IDiscountProvider>();
        providerMock.Setup(p => p.GetDiscountPercent("WELCOME10")).Returns(10m);
        var calc = new OrderCalculator(providerMock.Object);

        var order = MakeOrder(price: 100m, qty: 1, coupon: "WELCOME10");
        decimal total = calc.CalculateTotal(order);

        Assert.Equal(90m, total);
        providerMock.Verify(p => p.GetDiscountPercent("WELCOME10"), Times.Once);
    }

    [Fact]
    public void CalculateTotal_CouponNotFound_WrapsIntoInvalidCouponException()
    {
        var providerMock = new Mock<IDiscountProvider>();
        providerMock.Setup(p => p.GetDiscountPercent("BAD"))
                    .Throws(new KeyNotFoundException("Нет такого кода"));
        var calc = new OrderCalculator(providerMock.Object);

        var order = MakeOrder(price: 100m, qty: 1, coupon: "BAD");
        var ex = Assert.Throws<InvalidCouponException>(() => calc.CalculateTotal(order));

        Assert.Equal("BAD", ex.CouponCode);
    }

    [Theory]
    [InlineData(-5)]
    [InlineData(150)]
    public void CalculateTotal_PercentOutOfRange_Throws(decimal badPercent)
    {
        var providerMock = new Mock<IDiscountProvider>();
        providerMock.Setup(p => p.GetDiscountPercent(It.IsAny<string>())).Returns(badPercent);
        var calc = new OrderCalculator(providerMock.Object);

        var order = MakeOrder(price: 100m, qty: 1, coupon: "X");
        Assert.Throws<InvalidCouponException>(() => calc.CalculateTotal(order));
    }

    [Fact]
    public void CalculateTotal_CouponPlusTierBonus_CappedAt100Percent()
    {
        var providerMock = new Mock<IDiscountProvider>();
        providerMock.Setup(p => p.GetDiscountPercent("VIP50")).Returns(99m);
        var calc = new OrderCalculator(providerMock.Object);

        // subtotal = 1000 * 110 = 110 000 -> tier-бонус 7%, плюс купон 99% = 106% -> канается до 100%.
        var order = MakeOrder(price: 1000m, qty: 110, coupon: "VIP50");
        Assert.Equal(0m, calc.CalculateTotal(order));
    }

    [Fact]
    public void CalculateSubtotal_NullItems_Throws()
    {
        var calc = new OrderCalculator(Mock.Of<IDiscountProvider>());
        var order = new Order { Items = null! };
        Assert.Throws<ArgumentException>(() => calc.CalculateSubtotal(order));
    }

    [Fact]
    public void CalculateSubtotal_NegativeQuantity_Throws()
    {
        var calc = new OrderCalculator(Mock.Of<IDiscountProvider>());
        var order = MakeOrder(price: 100m, qty: -1);
        Assert.Throws<ArgumentException>(() => calc.CalculateSubtotal(order));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CalculateTotal_EmptyOrWhitespaceCoupon_NoDiscountApplied(string? coupon)
    {
        var calc = new OrderCalculator(Mock.Of<IDiscountProvider>());
        var order = MakeOrder(price: 100m, qty: 1, coupon: coupon);
        Assert.Equal(100m, calc.CalculateTotal(order));
    }
}