
using MaleFashion.Domain.Enums;
using MediatR;

namespace MaleFashion.Application.Features.Orders.Commands.UpdateOrderStatus;

public class UpdateOrderStatusCommand : IRequest
{
    public int OrderId { get; set; }
    public OrderStatus Status { get; set; }
}