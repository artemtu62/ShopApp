using Microsoft.EntityFrameworkCore;

namespace ShopApp.DAL.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly ShopDbContext _context;
    private readonly DbSet<T> _set;

    public GenericRepository(ShopDbContext context)
    {
        _context = context;
        _set = context.Set<T>();
    }

    public T? GetById(int id) => _set.Find(id);
    public IEnumerable<T> GetAll() => _set.AsNoTracking().ToList();

    public void Add(T entity) => _set.Add(entity);

    public void Update(T entity) => _context.Entry(entity).State = EntityState.Modified;

    public void Delete(int id)
    {
        T? entity = _set.Find(id);
        if (entity is not null) _set.Remove(entity);
    }

    public async Task<T?> GetByIdAsync(int id) => await _set.FindAsync(id);
    public async Task<IEnumerable<T>> GetAllAsync() => await _set.AsNoTracking().ToListAsync();
    public async Task AddAsync(T entity) => await _set.AddAsync(entity);

    public Task UpdateAsync(T entity)
    {
        Update(entity);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(int id)
    {
        T? entity = await _set.FindAsync(id);
        if (entity is not null) _set.Remove(entity);
    }
}