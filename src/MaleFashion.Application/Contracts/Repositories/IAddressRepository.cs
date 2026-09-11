using MaleFashion.Domain.Entities;

namespace MaleFashion.Application.Contracts.Repositories;

public interface IAddressRepository : IRepository<Address>
{
    Task<List<Address>> GetByUserIdAsync(string userId);
}