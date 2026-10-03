using Microsoft.EntityFrameworkCore;
using ShopApp.Common.Models;

namespace ShopApp.DAL.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ShopDbContext _context;

    public ProductRepository(ShopDbContext context)
    {
        _context = context;
    }

    public Product? GetById(int id) =>
        _context.Products.Include(p => p.Category).FirstOrDefault(p => p.Id == id);

    public IEnumerable<Product> GetAll() =>
        _context.Products.Include(p => p.Category).OrderBy(p => p.Id).AsNoTracking().ToList();

    public void Add(Product entity)
    {
        _context.Products.Add(entity);
        _context.SaveChanges();
    }

    public void Update(Product entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        Product? product = _context.Products.Find(id);
        if (product is null) return;
        _context.Products.Remove(product);
        _context.SaveChanges();
    }

    public IEnumerable<Product> GetByCategoryName(string categoryName) =>
        _context.Products
            .Include(p => p.Category)
            .Where(p => p.Category!.Name.Contains(categoryName))
            .OrderBy(p => p.Id)
            .AsNoTracking()
            .ToList();

    public IEnumerable<Product> GetByPriceRange(decimal min, decimal max) =>
        _context.Products
            .Include(p => p.Category)
            .Where(p => p.Price >= min && p.Price <= max)
            .OrderBy(p => p.Id)
            .AsNoTracking()
            .ToList();
}