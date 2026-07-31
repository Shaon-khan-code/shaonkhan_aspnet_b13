using Cortex.Mediator.Queries;
using Demo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Application.Features.Products.Query
{
    public class GetProductByIdQuery : IQuery<Product?>
    {
        public Guid Id { get; set; }
    }
}
