using DevSkill.Shop.Application.DTOs;
using DevSkill.Shop.Domain.Entities;
using DevSkill.Shop.Domain.Repositories;

namespace DevSkill.Shop.Application.Services
{
    // ── Interface ────────────────────────────────────────────────────────────
    public interface ICategoryService
    {
        Task<PagedResult<CategoryListDto>> GetPagedCategoriesAsync(
            int pageIndex, int pageSize, string? search, string? sortColumn, string? sortOrder);
        Task<CategoryUpdateDto?> GetByIdAsync(Guid id);
        Task CreateAsync(CategoryCreateDto dto);
        Task UpdateAsync(CategoryUpdateDto dto);
        Task DeleteAsync(Guid id);
        Task<IList<CategoryListDto>> GetAllActiveAsync();
    }

    // ── Implementation ───────────────────────────────────────────────────────
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repo;

        public CategoryService(ICategoryRepository repo) => _repo = repo;

        public async Task<PagedResult<CategoryListDto>> GetPagedCategoriesAsync(
            int pageIndex, int pageSize, string? search, string? sortColumn, string? sortOrder)
        {
            System.Linq.Expressions.Expression<Func<Category, bool>>? filter = null;
            if (!string.IsNullOrWhiteSpace(search))
                filter = c => c.Name.Contains(search);

            Func<IQueryable<Category>, IOrderedQueryable<Category>> orderBy = (sortColumn, sortOrder) switch
            {
                ("Name", "desc") => q => q.OrderByDescending(c => c.Name),
                ("IsActive", "asc") => q => q.OrderBy(c => c.IsActive),
                ("IsActive", "desc") => q => q.OrderByDescending(c => c.IsActive),
                ("Name", _) => q => q.OrderBy(c => c.Name),
                (_, "desc") => q => q.OrderByDescending(c => c.Id),
                _ => q => q.OrderBy(c => c.Id),
            };

            var (data, total, totalDisplay) = await _repo.GetPagedAsync(
                pageIndex, pageSize, filter, orderBy, "Products");

            return new PagedResult<CategoryListDto>
            {
                Total = total,
                TotalDisplay = totalDisplay,
                Data = data.Select(c => new CategoryListDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    IsActive = c.IsActive,
                    ImageName = c.ImageName,
                    ProductCount = c.Products.Count
                }).ToList()
            };
        }

        public async Task<CategoryUpdateDto?> GetByIdAsync(Guid id)
        {
            var c = await _repo.GetByIdAsync(id);
            if (c == null) return null;
            return new CategoryUpdateDto { Id = c.Id, Name = c.Name, IsActive = c.IsActive, ImageName = c.ImageName };
        }

        public async Task CreateAsync(CategoryCreateDto dto)
        {
            var entity = new Category { Id = Guid.NewGuid(), Name = dto.Name, IsActive = dto.IsActive, ImageName = dto.ImageName };
            await _repo.AddAsync(entity);
        }

        public async Task UpdateAsync(CategoryUpdateDto dto)
        {
            var entity = await _repo.GetByIdAsync(dto.Id)
                ?? throw new InvalidOperationException("Category not found.");
            entity.Name = dto.Name;
            entity.IsActive = dto.IsActive;
            entity.ImageName = dto.ImageName;
            await _repo.UpdateAsync(entity);
        }

        public async Task DeleteAsync(Guid id)
        {
            var hasProducts = await _repo.ExistsAsync(c => c.Id == id && c.Products.Any());
            if (hasProducts)
                throw new InvalidOperationException("Cannot delete a category that has products.");
            await _repo.DeleteAsync(id);
        }

        public async Task<IList<CategoryListDto>> GetAllActiveAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Where(c => c.IsActive)
                       .Select(c => new CategoryListDto { Id = c.Id, Name = c.Name })
                       .ToList();
        }
    }
}