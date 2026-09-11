using MaleFashion.Application.Common.Models;
using MaleFashion.Domain.Entities;

namespace MaleFashion.Application.Contracts.Repositories;

public interface IInventoryRepository : IRepository<Inventory>
{
    Task<Inventory?> GetByProductIdAsync(int productId);

    Task<(List<Inventory> Items, int TotalCount)> GetPagedAsync(
        bool? lowStockOnly,
        int pageNumber,
        int pageSize);
}