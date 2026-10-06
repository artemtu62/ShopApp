using System.Globalization;
using System.Text;
using Microsoft.EntityFrameworkCore;
using ShopApp.Common.Models;
using ShopApp.DAL;
using ShopApp.DAL.Repositories;

Console.OutputEncoding = Encoding.UTF8;
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
// Для локальной демонстрации репозиториев без установленного SQL Server используется
// SQLite — тот же EF Core API, меняется только провайдер в этой строке:
var options = new DbContextOptionsBuilder<ShopDbContext>()
    .UseSqlite("Data Source=shopapp.db")
    .Options;

Console.WriteLine("=== Часть 2. Паттерн «Репозиторий» на домене ShopApp (продолжение ЛР1) ===");

using (var context = new ShopDbContext(options))
{
    context.Database.EnsureDeleted();
    context.Database.EnsureCreated();

    context.Categories.AddRange(
        new Category { Name = "Электроника" },
        new Category { Name = "Книги" });
    context.SaveChanges();

    IProductRepository products = new ProductRepository(context);

    Console.WriteLine("[1] Add — добавляем 5 товаров через ProductRepository");
    products.Add(new Product { Name = "Смартфон X200", Price = 45000m, Description = "6.5\", 128 ГБ", CategoryId = 1 });
    products.Add(new Product { Name = "Наушники BT-100", Price = 3200m, Description = "Беспроводные", CategoryId = 1 });
    products.Add(new Product { Name = "Ноутбук ProBook 14", Price = 68000m, Description = "14\", 16 ГБ RAM", CategoryId = 1 });
    products.Add(new Product { Name = "Война и мир", Price = 1200m, Description = "Л.Н. Толстой", CategoryId = 2 });
    products.Add(new Product { Name = "Чистый код", Price = 2400m, Description = "Р. Мартин", CategoryId = 2 });

    Console.WriteLine("[2] GetAll — все товары:");
    foreach (Product product in products.GetAll())
    {
        PrintProduct(product);
    }

    Console.WriteLine("[3] GetById(3):");
    Product? found = products.GetById(3);
    if (found is not null) PrintProduct(found);

    Console.WriteLine("[4] Update — меняем цену товара №3 на 64000:");
    if (found is not null)
    {
        found.Price = 64000m;
        products.Update(found);
    }
    PrintProduct(products.GetById(3)!);

    Console.WriteLine("[5] GetByCategoryName(\"Книги\"):");
    foreach (Product product in products.GetByCategoryName("Книги"))
    {
        PrintProduct(product);
    }

    Console.WriteLine("[6] GetByPriceRange(1000, 5000):");
    foreach (Product product in products.GetByPriceRange(1000m, 5000m))
    {
        PrintProduct(product);
    }

    Console.WriteLine("[7] Delete(2):");
    products.Delete(2);
    Console.WriteLine($"  Товаров в базе осталось: {products.GetAll().Count()}");
}

Console.WriteLine("[8] Unit of Work + асинхронные методы:");
using (var uow = new UnitOfWork(new ShopDbContext(options)))
{
    var client = new Client
    {
        FullName = "Иванов Иван",
        Email = "ivanov@example.com",
        Phone = "+7 900 000-00-00",
        Address = "г. Москва"
    };

    // Клиент и заказ сохраняются одной транзакцией через один CompleteAsync().
    await uow.Clients.AddAsync(client);
    await uow.Orders.AddAsync(new Order
    {
        Client = client,
        OrderDate = new DateTime(2026, 9, 30),
        Status = OrderStatus.Новый,
        Total = 64000m
    });

    int saved = await uow.CompleteAsync();
    Console.WriteLine($"  Сохранено записей одной транзакцией: {saved}");

    foreach (Order order in await uow.Orders.GetAllAsync())
    {
        Console.WriteLine(
            $"  Заказ #{order.Id}: клиент #{order.ClientId}, дата {order.OrderDate:yyyy-MM-dd}, сумма {order.Total:F2}");
    }
}

Console.WriteLine("[9] ClientRepository.FindByEmail(\"ivanov@example.com\"):");
using (var context = new ShopDbContext(options))
{
    IClientRepository clients = new ClientRepository(context);
    Client? client = clients.FindByEmail("ivanov@example.com");
    Console.WriteLine(client is null
        ? "  Клиент не найден"
        : $"  Клиент #{client.Id}: {client.FullName}");
}

static void PrintProduct(Product product) =>
    Console.WriteLine($"  #{product.Id} {product.Name} — {product.Category?.Name}, {product.Price:F2}");