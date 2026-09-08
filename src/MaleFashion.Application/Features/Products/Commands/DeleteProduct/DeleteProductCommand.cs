// DeleteProductCommand.cs
using MediatR;

namespace MaleFashion.Application.Features.Products.Commands.DeleteProduct;

public class DeleteProductCommand : IRequest
{
    public int Id { get; set; }
}