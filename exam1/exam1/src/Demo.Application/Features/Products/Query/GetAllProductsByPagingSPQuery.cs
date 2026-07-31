using Cortex.Mediator.Queries;
using Demo.Domain.DTOs;
using Demo.Domain.Entities;
using Demo.Domain.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Application.Features.Products.Query
{
    public class GetAllProductsByPagingSPQuery : DataTables, IQuery<(IList<PagedProductListDTO>, int, int)>
    {
        public string Name { get; set; } = null!;
        public double? PriceFrom { get; set; }
        public double? PriceTo { get; set; }
        public string? SortText { get; set; }
    }
}
