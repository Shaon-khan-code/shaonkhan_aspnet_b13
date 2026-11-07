using MaleFashion.Application.Features.Products.Queries.GetProductList;
using MaleFashion.Domain.Entities;

namespace MaleFashion.Application.Contracts.Repositories;

public interface IProductRepository : IRepository<Product>
{
    Task<(List<Product> Items, int TotalCount)> GetPagedAsync(
        string? searchTerm,
        int? categoryId,
        bool? isActive,
        decimal? minPrice,
        decimal? maxPrice,
        ProductSortOption sortBy,
        int pageNumber,
        int pageSize);

    Task<Product?> GetByIdWithCategoryAsync(int id);

   
    Task<Product?> GetByIdWithInventoryAsync(int id);

    Task<Product?> GetDetailsAsync(int id);

    Task<List<Product>> GetRelatedProductsAsync(int categoryId, int excludeProductId, int count);

    Task<bool> HasOrderItemsAsync(int productId);
}
