using Demo.Application.Contracts.Repositories;
using Demo.Application.Features.Products.Query;
using Demo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Infrastructure.Data.Repositories
{
    public class ProductRepository : Repository<Product, Guid>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<(IList<Product>, int, int)> GetPagedProducts(GetAllProductsByPagingQuery query,
            CancellationToken cancellationToken)
        {
            return await GetDynamicAsync(x => query.SearchText == null || x.Name.Contains(query.SearchText), 
                query.SortText, 
                null, 
                query.PageIndex, 
                query.PageSize, 
                true, 
                cancellationToken);
        }

        public async Task<bool> IsDuplicateProductName(string name, Guid? id, CancellationToken cancellationToken)
        {
            if(!id.HasValue) 
                return (await GetCountAsync(x => x.Name == name, cancellationToken)) > 0;
            else
                return (await GetCountAsync(x => x.Name == name && x.Id != id.Value, cancellationToken)) > 0;
        }
    }
}
