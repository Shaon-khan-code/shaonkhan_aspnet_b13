using Cortex.Mediator.Queries;
using Demo.Application.Contracts;
using Demo.Domain.DTOs;
using Demo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Application.Features.Products.Query
{
    public class GetAllProductsByPagingSPQueryHandler : 
        IQueryHandler<GetAllProductsByPagingSPQuery, (IList<PagedProductListDTO>, int, int)>
    {
        private readonly IApplicationUnitOfWork _applicationUnitOfWork;

        public GetAllProductsByPagingSPQueryHandler(IApplicationUnitOfWork applicationUnitOfWork)
        {
            _applicationUnitOfWork = applicationUnitOfWork;
        }

        public async Task<(IList<PagedProductListDTO>, int, int)> Handle(GetAllProductsByPagingSPQuery query, 
            CancellationToken cancellationToken)
        {
            var storedProcedureName = "GetProducts";

            var output = await _applicationUnitOfWork.SqlUtility
                .QueryWithStoredProcedureAsync<PagedProductListDTO>(storedProcedureName,
                new Dictionary<string, object?>
                {
                    { "Name", query.Name },
                    { "PriceFrom", query.PriceFrom },
                    { "PriceTo", query.PriceTo },
                    { "PageIndex", query.PageIndex },
                    { "PageSize", query.PageSize },
                    { "OrderBy", query.SortText }
                },
                new Dictionary<string, Type>
                {
                    { "Total", typeof(int) },
                    { "TotalDisplay", typeof(int) }
                });

            return (output.result, (int)output.outValues["Total"], (int)output.outValues["TotalDisplay"]);
        }
    }
}
