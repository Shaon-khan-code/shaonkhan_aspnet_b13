using MaleFashion.Application.Contracts.Repositories;
using MaleFashion.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MaleFashion.Infrastructure.Data.Repositories;

public class AddressRepository : Repository<Address>, IAddressRepository
{
    public AddressRepository(ApplicationDbContext context) : base(context) { }

    public async Task<List<Address>> GetByUserIdAsync(string userId)
    {
        return await DbSet.AsNoTracking().Where(a => a.UserId == userId).ToListAsync();
    }
}