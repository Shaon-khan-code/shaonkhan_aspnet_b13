using Demo.Application.Features.Products.Query;
using Demo.Domain.Contracts;
using Demo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Application.Contracts.Repositories
{
    public interface IProductRepository : IRepository<Product, Guid>
    {
        Task<(IList<Product>, int, int)> GetPagedProducts(GetAllProductsByPagingQuery query, 
            CancellationToken cancellationToken);
        Task<bool> IsDuplicateProductName(string name, Guid? Id, CancellationToken cancellationToken);
    }
}
