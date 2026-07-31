using Cortex.Mediator.Queries;
using Demo.Application.Contracts;
using Demo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Application.Features.Products.Query
{
    public class GetProductByIdQueryHandler : IQueryHandler<GetProductByIdQuery, Product?>
    {
        private readonly IApplicationUnitOfWork _applicationUnitOfWork;

        public GetProductByIdQueryHandler(IApplicationUnitOfWork applicationUnitOfWork)
        {
            _applicationUnitOfWork = applicationUnitOfWork;
        }

        public async Task<Product?> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
        {
            return await _applicationUnitOfWork.ProductRepository.GetByIdAsync(query.Id, cancellationToken);
        }
    }
}
