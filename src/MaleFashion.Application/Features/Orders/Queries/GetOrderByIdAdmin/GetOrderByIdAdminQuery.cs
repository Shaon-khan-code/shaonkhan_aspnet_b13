using MaleFashion.Application.DTOs;
using MediatR;

namespace MaleFashion.Application.Features.Orders.Queries.GetOrderByIdAdmin;

public class GetOrderByIdAdminQuery : IRequest<OrderDto>
{
    public int OrderId { get; set; }
}