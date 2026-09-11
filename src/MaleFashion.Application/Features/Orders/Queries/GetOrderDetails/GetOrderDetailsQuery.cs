using MaleFashion.Application.DTOs;
using MediatR;

namespace MaleFashion.Application.Features.Orders.Queries.GetOrderDetails;

public class GetOrderDetailsQuery : IRequest<OrderDto>
{
    public int OrderId { get; set; }
    public string UserId { get; set; } = string.Empty;
}