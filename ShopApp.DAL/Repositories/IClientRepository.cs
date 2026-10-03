using ShopApp.Common.Models;

namespace ShopApp.DAL.Repositories;

public interface IClientRepository : IRepository<Client>
{
    Client? FindByEmail(string email);
}