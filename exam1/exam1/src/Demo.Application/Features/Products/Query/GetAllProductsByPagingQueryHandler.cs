using Cortex.Mediator.Queries;
using Demo.Application.Contracts;
using Demo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Application.Features.Products.Query
{
    public class GetAllProductsByPagingQueryHandler : IQueryHandler<GetAllProductsByPagingQuery, (IList<Product>, int, int)>
    {
        private readonly IApplicationUnitOfWork _applicationUnitOfWork;

        public GetAllProductsByPagingQueryHandler(IApplicationUnitOfWork applicationUnitOfWork)
        {
            _applicationUnitOfWork = applicationUnitOfWork;
        }

        public async Task<(IList<Product>, int, int)> Handle(GetAllProductsByPagingQuery query, 
            CancellationToken cancellationToken)
        {
            return await _applicationUnitOfWork.ProductRepository.GetPagedProducts(query, cancellationToken);
        }
    }
}
