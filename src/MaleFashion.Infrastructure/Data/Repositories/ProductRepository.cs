using MaleFashion.Application.Contracts.Repositories;
using MaleFashion.Application.Features.Products.Queries.GetProductList;
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
        decimal? minPrice,
        decimal? maxPrice,
        ProductSortOption sortBy,
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

        if (minPrice.HasValue)
        {
            query = query.Where(p => p.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= maxPrice.Value);
        }

        query = sortBy switch
        {
            ProductSortOption.NameDesc => query.OrderByDescending(p => p.Name),
            ProductSortOption.PriceAsc => query.OrderBy(p => p.Price),
            ProductSortOption.PriceDesc => query.OrderByDescending(p => p.Price),
            ProductSortOption.Newest => query.OrderByDescending(p => p.CreatedAt),
            _ => query.OrderBy(p => p.Name)
        };

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Product?> GetByIdWithCategoryAsync(int id)
    {
        return await DbSet.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
    }

    // নতুন: Add to Cart এর জন্য, স্টকসহ প্রোডাক্ট আনে
    public async Task<Product?> GetByIdWithInventoryAsync(int id)
    {
        return await DbSet
            .Include(p => p.Inventory)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<bool> HasOrderItemsAsync(int productId)
    {
        return await Context.Set<OrderItem>().AnyAsync(oi => oi.ProductId == productId);
    }

    public async Task<Product?> GetDetailsAsync(int id)
    {
        return await DbSet
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.Inventory)
            .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);
    }

    public async Task<List<Product>> GetRelatedProductsAsync(int categoryId, int excludeProductId, int count)
    {
        return await DbSet
            .AsNoTracking()
            .Include(p => p.Category)
            .Where(p => p.CategoryId == categoryId && p.Id != excludeProductId && p.IsActive)
            .OrderByDescending(p => p.CreatedAt)
            .Take(count)
            .ToListAsync();
    }
}
