// GetProductByIdQuery.cs
using MaleFashion.Application.DTOs;
using MediatR;

namespace MaleFashion.Application.Features.Products.Queries.GetProductById;

public class GetProductByIdQuery : IRequest<ProductDto>
{
    public int Id { get; set; }
}