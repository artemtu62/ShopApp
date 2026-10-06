using Moq;
using ShopApp.BLL;
using ShopApp.Common.Models;
using Xunit;

namespace ShopApp.BLL.Tests;

public class StockServiceTests
{
    // --- ReserveStock ---

    [Theory]
    [InlineData(0)]
    [InlineData(-3)]
    public void ReserveStock_NonPositiveQuantity_Throws(int qty)
    {
        var repo = new Mock<IProductStockRepository>();
        var service = new StockService(repo.Object);

        Assert.Throws<ArgumentOutOfRangeException>(() => service.ReserveStock(1, qty));
        repo.Verify(r => r.GetById(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public void ReserveStock_ProductNotFound_Throws()
    {
        var repo = new Mock<IProductStockRepository>();
        repo.Setup(r => r.GetById(42)).Returns((Product?)null);
        var service = new StockService(repo.Object);

        var ex = Assert.Throws<ProductNotFoundException>(() => service.ReserveStock(42, 1));
        Assert.Equal(42, ex.ProductId);
    }

    [Fact]
    public void ReserveStock_InsufficientStock_Throws()
    {
        var repo = new Mock<IProductStockRepository>();
        repo.Setup(r => r.GetById(1)).Returns(new Product { Id = 1, StockQuantity = 2 });
        var service = new StockService(repo.Object);

        var ex = Assert.Throws<InsufficientStockException>(() => service.ReserveStock(1, 5));

        Assert.Equal(1, ex.ProductId);   // ← покрывает строку 16
        Assert.Equal(5, ex.Requested);   // ← покрывает строку 17
        Assert.Equal(2, ex.Available);   // ← уже покрыто

        repo.Verify(r => r.Update(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public void ReserveStock_EnoughStock_DecrementsAndUpdates()
    {
        var product = new Product { Id = 1, StockQuantity = 10 };
        var repo = new Mock<IProductStockRepository>();
        repo.Setup(r => r.GetById(1)).Returns(product);
        var service = new StockService(repo.Object);

        service.ReserveStock(1, 4);

        Assert.Equal(6, product.StockQuantity);
        repo.Verify(r => r.Update(product), Times.Once);
    }
    [Fact]
    public void Constructor_NullRepository_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new StockService(null!));
    }

    // --- ReleaseStock ---

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void ReleaseStock_NonPositiveQuantity_Throws(int qty)
    {
        var repo = new Mock<IProductStockRepository>();
        var service = new StockService(repo.Object);
        Assert.Throws<ArgumentOutOfRangeException>(() => service.ReleaseStock(1, qty));
    }

    [Fact]
    public void ReleaseStock_ProductNotFound_Throws()
    {
        var repo = new Mock<IProductStockRepository>();
        repo.Setup(r => r.GetById(1)).Returns((Product?)null);
        var service = new StockService(repo.Object);

        Assert.Throws<ProductNotFoundException>(() => service.ReleaseStock(1, 3));
    }

    [Fact]
    public void ReleaseStock_Valid_IncrementsAndUpdates()
    {
        var product = new Product { Id = 1, StockQuantity = 5 };
        var repo = new Mock<IProductStockRepository>();
        repo.Setup(r => r.GetById(1)).Returns(product);
        var service = new StockService(repo.Object);

        service.ReleaseStock(1, 3);

        Assert.Equal(8, product.StockQuantity);
        repo.Verify(r => r.Update(product), Times.Once);
    }

    // --- HasEnoughStock ---

    [Fact]
    public void HasEnoughStock_ProductNotFound_Throws()
    {
        var repo = new Mock<IProductStockRepository>();
        repo.Setup(r => r.GetById(1)).Returns((Product?)null);
        var service = new StockService(repo.Object);

        Assert.Throws<ProductNotFoundException>(() => service.HasEnoughStock(1, 1));
    }

    [Theory]
    [InlineData(5, 5, true)]
    [InlineData(5, 6, false)]
    public void HasEnoughStock_ReturnsExpected(int available, int requested, bool expected)
    {
        var repo = new Mock<IProductStockRepository>();
        repo.Setup(r => r.GetById(1)).Returns(new Product { Id = 1, StockQuantity = available });
        var service = new StockService(repo.Object);

        Assert.Equal(expected, service.HasEnoughStock(1, requested));
    }
}