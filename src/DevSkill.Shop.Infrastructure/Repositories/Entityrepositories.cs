using DevSkill.Shop.Domain.Entities;
using DevSkill.Shop.Domain.Repositories;
using DevSkill.Shop.Infrastructure.Data;

namespace DevSkill.Shop.Infrastructure.Repositories
{
    public class CategoryRepository : Repository<Category, Guid>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context) : base(context) { }
    }

    public class ProductRepository : Repository<Product, Guid>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context) { }
    }

    public class ProductImageRepository : Repository<ProductImage, Guid>, IProductImageRepository
    {
        public ProductImageRepository(ApplicationDbContext context) : base(context) { }
    }

    public class StockRepository : Repository<Stock, Guid>, IStockRepository
    {
        public StockRepository(ApplicationDbContext context) : base(context) { }
    }
}