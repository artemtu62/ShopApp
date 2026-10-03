using Microsoft.EntityFrameworkCore;
using ShopApp.Common.Models;

namespace ShopApp.DAL.Repositories;

public class ClientRepository : IClientRepository
{
    private readonly ShopDbContext _context;

    public ClientRepository(ShopDbContext context)
    {
        _context = context;
    }

    public Client? GetById(int id) => _context.Clients.Find(id);

    public IEnumerable<Client> GetAll() =>
        _context.Clients.OrderBy(c => c.Id).AsNoTracking().ToList();

    public void Add(Client entity)
    {
        _context.Clients.Add(entity);
        _context.SaveChanges();
    }

    public void Update(Client entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        Client? client = _context.Clients.Find(id);
        if (client is null) return;
        _context.Clients.Remove(client);
        _context.SaveChanges();
    }

    public Client? FindByEmail(string email) =>
        _context.Clients.FirstOrDefault(c => c.Email == email);
}