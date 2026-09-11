using MaleFashion.Application.DTOs;
using MediatR;

namespace MaleFashion.Application.Features.Products.Queries.GetProductDetails;

public class GetProductDetailsQuery : IRequest<ProductDetailsDto>
{
    public int Id { get; set; }
}