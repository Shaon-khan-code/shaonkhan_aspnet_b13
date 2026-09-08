using MaleFashion.Domain.Entities;

namespace MaleFashion.Application.Contracts.Repositories;

public interface IProductRepository : IRepository<Product>
{
    Task<(List<Product> Items, int TotalCount)> GetPagedAsync(
        string? searchTerm,
        int? categoryId,
        bool? isActive,
        int pageNumber,
        int pageSize);
}