using DevSkill.Shop.Application.DTOs;
using DevSkill.Shop.Domain.Entities;
using DevSkill.Shop.Domain.Repositories;

namespace DevSkill.Shop.Application.Services
{
    public interface IProductService
    {
        Task<PagedResult<ProductListDto>> GetPagedProductsAsync(
            int pageIndex, int pageSize, string? search, string? sortColumn, string? sortOrder);
        Task<ProductUpdateDto?> GetByIdAsync(Guid id);
        Task CreateAsync(ProductCreateDto dto);
        Task UpdateAsync(ProductUpdateDto dto);
        Task DeleteAsync(Guid id);
    }

    public class ProductService : IProductService
    {
        private readonly IProductRepository _repo;

        public ProductService(IProductRepository repo) => _repo = repo;

        public async Task<PagedResult<ProductListDto>> GetPagedProductsAsync(
            int pageIndex, int pageSize, string? search, string? sortColumn, string? sortOrder)
        {
            System.Linq.Expressions.Expression<Func<Product, bool>>? filter = null;
            if (!string.IsNullOrWhiteSpace(search))
                filter = p => p.Name.Contains(search) ||
                              (p.Brand != null && p.Brand.Contains(search)) ||
                              (p.Sku != null && p.Sku.Contains(search));

            Func<IQueryable<Product>, IOrderedQueryable<Product>> orderBy = (sortColumn, sortOrder) switch
            {
                ("Name", "desc") => q => q.OrderByDescending(p => p.Name),
                ("Price", "asc") => q => q.OrderBy(p => p.Price),
                ("Price", "desc") => q => q.OrderByDescending(p => p.Price),
                ("Category", "asc") => q => q.OrderBy(p => p.Category.Name),
                ("Category", "desc") => q => q.OrderByDescending(p => p.Category.Name),
                ("Name", _) => q => q.OrderBy(p => p.Name),
                (_, "desc") => q => q.OrderByDescending(p => p.Id),
                _ => q => q.OrderBy(p => p.Id),
            };

            var (data, total, totalDisplay) = await _repo.GetPagedAsync(
                pageIndex, pageSize, filter, orderBy, "Category,Stock");

            return new PagedResult<ProductListDto>
            {
                Total = total,
                TotalDisplay = totalDisplay,
                Data = data.Select(p => new ProductListDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    IsActive = p.IsActive,
                    CategoryName = p.Category?.Name,
                    Brand = p.Brand,
                    Sku = p.Sku,
                    Color = p.Color,
                    Weight = p.Weight,
                    StockQuantity = p.Stock?.Quantity
                }).ToList()
            };
        }

        public async Task<ProductUpdateDto?> GetByIdAsync(Guid id)
        {
            var p = await _repo.GetByIdAsync(id);
            if (p == null) return null;
            return new ProductUpdateDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                IsActive = p.IsActive,
                CategoryId = p.CategoryId,
                Brand = p.Brand,
                Sku = p.Sku,
                Color = p.Color,
                Weight = p.Weight
            };
        }

        public async Task CreateAsync(ProductCreateDto dto)
        {
            var entity = new Product
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Price = dto.Price,
                IsActive = dto.IsActive,
                CategoryId = dto.CategoryId,
                Brand = dto.Brand,
                Sku = dto.Sku,
                Color = dto.Color,
                Weight = dto.Weight
            };
            await _repo.AddAsync(entity);
        }

        public async Task UpdateAsync(ProductUpdateDto dto)
        {
            var entity = await _repo.GetByIdAsync(dto.Id)
                ?? throw new InvalidOperationException("Product not found.");
            entity.Name = dto.Name; entity.Price = dto.Price; entity.IsActive = dto.IsActive;
            entity.CategoryId = dto.CategoryId; entity.Brand = dto.Brand;
            entity.Sku = dto.Sku; entity.Color = dto.Color; entity.Weight = dto.Weight;
            await _repo.UpdateAsync(entity);
        }

        public async Task DeleteAsync(Guid id) => await _repo.DeleteAsync(id);
    }
}