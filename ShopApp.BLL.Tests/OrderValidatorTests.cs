using Moq;
using ShopApp.BLL;
using ShopApp.Common.Models;
using Xunit;

namespace ShopApp.BLL.Tests;

public class OrderValidatorTests
{
    private static Mock<IClientExistenceChecker> ExistingClientChecker()
    {
        var mock = new Mock<IClientExistenceChecker>();
        mock.Setup(c => c.Exists(It.IsAny<int>())).Returns(true);
        return mock;
    }

    // --- Validate ---

    [Fact]
    public void Validate_NullOrder_Throws()
    {
        var validator = new OrderValidator(ExistingClientChecker().Object);
        Assert.Throws<ArgumentNullException>(() => validator.Validate(null!));
    }

    [Fact]
    public void Validate_UnknownClient_AddsError()
    {
        var checker = new Mock<IClientExistenceChecker>();
        checker.Setup(c => c.Exists(99)).Returns(false);
        var validator = new OrderValidator(checker.Object);

        var order = new Order
        {
            ClientId = 99,
            Items = new List<OrderItem> { new() { ProductId = 1, Quantity = 1, Price = 10 } }
        };

        List<string> errors = validator.Validate(order);
        Assert.Contains(errors, e => e.Contains("Клиент"));
    }

    [Fact]
    public void Validate_EmptyItems_AddsError()
    {
        var validator = new OrderValidator(ExistingClientChecker().Object);
        var order = new Order { ClientId = 1, Items = new List<OrderItem>() };

        List<string> errors = validator.Validate(order);
        Assert.Contains(errors, e => e.Contains("хотя бы одну позицию"));
    }

    [Fact]
    public void Validate_NonPositiveQuantity_AddsError()
    {
        var validator = new OrderValidator(ExistingClientChecker().Object);
        var order = new Order
        {
            ClientId = 1,
            Items = new List<OrderItem> { new() { ProductId = 1, Quantity = 0, Price = 10 } }
        };

        List<string> errors = validator.Validate(order);
        Assert.Contains(errors, e => e.Contains("количество"));
    }

    [Fact]
    public void Validate_NegativePrice_AddsError()
    {
        var validator = new OrderValidator(ExistingClientChecker().Object);
        var order = new Order
        {
            ClientId = 1,
            Items = new List<OrderItem> { new() { ProductId = 1, Quantity = 1, Price = -5 } }
        };

        List<string> errors = validator.Validate(order);
        Assert.Contains(errors, e => e.Contains("цена"));
    }

    [Fact]
    public void Validate_ValidOrder_NoErrors()
    {
        var validator = new OrderValidator(ExistingClientChecker().Object);
        var order = new Order
        {
            ClientId = 1,
            Items = new List<OrderItem> { new() { ProductId = 1, Quantity = 2, Price = 100 } }
        };

        List<string> errors = validator.Validate(order);
        Assert.Empty(errors);
    }

    [Fact]
    public void Constructor_NullChecker_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new OrderValidator(null!));
    }

    [Fact]
    public void Validate_NullItems_AddsError()
    {
        var validator = new OrderValidator(ExistingClientChecker().Object);
        var order = new Order { ClientId = 1, Items = null! };

        var errors = validator.Validate(order);
        Assert.Contains(errors, e => e.Contains("хотя бы одну позицию"));
    }

    [Fact]
    public void Validate_NegativeQuantity_AddsError()
    {
        var validator = new OrderValidator(ExistingClientChecker().Object);
        var order = new Order
        {
            ClientId = 1,
            Items = new List<OrderItem> { new() { ProductId = 1, Quantity = -1, Price = 10 } }
        };

        var errors = validator.Validate(order);
        Assert.Contains(errors, e => e.Contains("количество"));
    }

    // --- EnsureCanTransition — все ветви switch ---

    [Theory]
    [InlineData(OrderStatus.Новый, OrderStatus.Оплачен)]
    [InlineData(OrderStatus.Новый, OrderStatus.Отменён)]
    [InlineData(OrderStatus.Оплачен, OrderStatus.Доставляется)]
    [InlineData(OrderStatus.Оплачен, OrderStatus.Отменён)]
    [InlineData(OrderStatus.Доставляется, OrderStatus.Доставлен)]
    public void EnsureCanTransition_AllowedTransitions_DoNotThrow(OrderStatus from, OrderStatus to)
    {
        var validator = new OrderValidator(ExistingClientChecker().Object);
        validator.EnsureCanTransition(from, to);
    }

    [Theory]
    [InlineData(OrderStatus.Новый, OrderStatus.Доставлен)]
    [InlineData(OrderStatus.Оплачен, OrderStatus.Новый)]
    [InlineData(OrderStatus.Доставляется, OrderStatus.Отменён)]
    [InlineData(OrderStatus.Доставлен, OrderStatus.Новый)]
    [InlineData(OrderStatus.Отменён, OrderStatus.Новый)]
    public void EnsureCanTransition_DisallowedTransitions_Throw(OrderStatus from, OrderStatus to)
    {
        var validator = new OrderValidator(ExistingClientChecker().Object);
        Assert.Throws<InvalidOperationException>(() => validator.EnsureCanTransition(from, to));
    }

    [Fact]
    public void EnsureCanTransition_UnknownEnumValue_ThrowsArgumentOutOfRange()
    {
        var validator = new OrderValidator(ExistingClientChecker().Object);
        var invalidStatus = (OrderStatus)999;

        Assert.Throws<ArgumentOutOfRangeException>(
            () => validator.EnsureCanTransition(invalidStatus, OrderStatus.Оплачен));
    }
}