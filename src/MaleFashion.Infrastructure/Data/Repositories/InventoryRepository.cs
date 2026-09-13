using MaleFashion.Application.Contracts.Repositories;
using MaleFashion.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MaleFashion.Infrastructure.Data.Repositories;

public class InventoryRepository : Repository<Inventory>, IInventoryRepository
{
    public InventoryRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Inventory?> GetByProductIdAsync(int productId)
    {
        return await DbSet.Include(i => i.Product)
            .FirstOrDefaultAsync(i => i.ProductId == productId);
    }
    public async Task<int> GetLowStockCountAsync()
    {
        return await DbSet
            .Where(i => i.QuantityInStock - i.ReservedQuantity <= i.ReorderLevel)
            .CountAsync();
    }

    public async Task<(List<Inventory> Items, int TotalCount)> GetPagedAsync(
        bool? lowStockOnly, int pageNumber, int pageSize)
    {
        var query = DbSet.AsNoTracking().Include(i => i.Product).AsQueryable();

        if (lowStockOnly == true)
        {
            // SQL-translatable equivalent of Inventory.IsLowStock
            // (AvailableQuantity <= ReorderLevel), since the computed
            // property itself cannot be translated to SQL.
            query = query.Where(i => i.QuantityInStock - i.ReservedQuantity <= i.ReorderLevel);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(i => i.Product.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}