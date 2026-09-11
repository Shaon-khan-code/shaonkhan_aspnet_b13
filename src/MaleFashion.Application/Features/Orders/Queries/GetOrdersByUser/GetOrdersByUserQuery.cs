using MaleFashion.Application.DTOs;
using MediatR;

namespace MaleFashion.Application.Features.Orders.Queries.GetOrdersByUser;

public class GetOrdersByUserQuery : IRequest<List<OrderSummaryDto>>
{
    public string UserId { get; set; } = string.Empty;
}