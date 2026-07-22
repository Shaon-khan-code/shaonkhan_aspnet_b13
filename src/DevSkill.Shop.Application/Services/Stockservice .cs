using DevSkill.Shop.Application.DTOs;
using DevSkill.Shop.Domain.Entities;
using DevSkill.Shop.Domain.Repositories;

namespace DevSkill.Shop.Application.Services
{
    public interface IStockService
    {
        Task<PagedResult<StockListDto>> GetPagedStocksAsync(
            int pageIndex, int pageSize, string? search, string? sortColumn, string? sortOrder);
        Task<StockUpdateDto?> GetByIdAsync(Guid id);
        Task CreateAsync(StockCreateDto dto);
        Task UpdateAsync(StockUpdateDto dto);
        Task DeleteAsync(Guid id);
    }

    public class StockService : IStockService
    {
        private readonly IStockRepository _repo;

        public StockService(IStockRepository repo) => _repo = repo;

        public async Task<PagedResult<StockListDto>> GetPagedStocksAsync(
            int pageIndex, int pageSize, string? search, string? sortColumn, string? sortOrder)
        {
            System.Linq.Expressions.Expression<Func<Stock, bool>>? filter = null;
            if (!string.IsNullOrWhiteSpace(search))
                filter = s => s.Product.Name.Contains(search);

            Func<IQueryable<Stock>, IOrderedQueryable<Stock>> orderBy = (sortColumn, sortOrder) switch
            {
                ("Product", "asc") => q => q.OrderBy(s => s.Product.Name),
                ("Product", "desc") => q => q.OrderByDescending(s => s.Product.Name),
                ("Quantity", "asc") => q => q.OrderBy(s => s.Quantity),
                ("Quantity", "desc") => q => q.OrderByDescending(s => s.Quantity),
                ("LowStockQuantity", "asc") => q => q.OrderBy(s => s.LowStockQuantity),
                ("LowStockQuantity", "desc") => q => q.OrderByDescending(s => s.LowStockQuantity),
                (_, "desc") => q => q.OrderByDescending(s => s.Id),
                _ => q => q.OrderBy(s => s.Id),
            };

            var (data, total, totalDisplay) = await _repo.GetPagedAsync(
                pageIndex, pageSize, filter, orderBy, "Product");

            return new PagedResult<StockListDto>
            {
                Total = total,
                TotalDisplay = totalDisplay,
                Data = data.Select(s => new StockListDto
                {
                    Id = s.Id,
                    ProductId = s.ProductId,
                    ProductName = s.Product?.Name,
                    Quantity = s.Quantity,
                    LowStockQuantity = s.LowStockQuantity
                }).ToList()
            };
        }

        public async Task<StockUpdateDto?> GetByIdAsync(Guid id)
        {
            var s = await _repo.GetByIdAsync(id);
            if (s == null) return null;
            return new StockUpdateDto { Id = s.Id, ProductId = s.ProductId, Quantity = s.Quantity, LowStockQuantity = s.LowStockQuantity };
        }

        public async Task CreateAsync(StockCreateDto dto)
        {
            bool exists = await _repo.ExistsAsync(s => s.ProductId == dto.ProductId);
            if (exists) throw new InvalidOperationException("A stock entry already exists for this product.");
            await _repo.AddAsync(new Stock { Id = Guid.NewGuid(), ProductId = dto.ProductId, Quantity = dto.Quantity, LowStockQuantity = dto.LowStockQuantity });
        }

        public async Task UpdateAsync(StockUpdateDto dto)
        {
            var entity = await _repo.GetByIdAsync(dto.Id)
                ?? throw new InvalidOperationException("Stock entry not found.");
            entity.ProductId = dto.ProductId;
            entity.Quantity = dto.Quantity;
            entity.LowStockQuantity = dto.LowStockQuantity;
            await _repo.UpdateAsync(entity);
        }

        public async Task DeleteAsync(Guid id) => await _repo.DeleteAsync(id);
    }
}