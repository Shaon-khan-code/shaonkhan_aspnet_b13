using DevSkill.Shop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevSkill.Shop.Domain.Repositories
{
    public interface ICategoryRepository : IRepository<Category, Guid> { }
    public interface IProductRepository : IRepository<Product, Guid> { }
    public interface IProductImageRepository : IRepository<ProductImage, Guid> { }
    public interface IStockRepository : IRepository<Stock, Guid> { }
}
