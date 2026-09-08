using MaleFashion.Application.Contracts.Repositories;
using MaleFashion.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MaleFashion.Infrastructure.Data.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<(List<Product> Items, int TotalCount)> GetPagedAsync(
        string? searchTerm,
        int? categoryId,
        bool? isActive,
        int pageNumber,
        int pageSize)
    {
        var query = DbSet.AsNoTracking().Include(p => p.Category).AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(p => EF.Functions.Like(p.Name, $"%{searchTerm}%"));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        if (isActive.HasValue)
        {
            query = query.Where(p => p.IsActive == isActive.Value);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(p => p.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Product?> GetByIdWithCategoryAsync(int id)
    {
        return await DbSet.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<bool> HasOrderItemsAsync(int productId)
    {
        return await Context.Set<OrderItem>().AnyAsync(oi => oi.ProductId == productId);
    }
}