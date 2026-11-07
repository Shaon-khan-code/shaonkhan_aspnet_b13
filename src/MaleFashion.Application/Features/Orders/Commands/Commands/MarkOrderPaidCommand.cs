using MediatR;

namespace MaleFashion.Application.Features.Orders.Commands.MarkOrderPaid;

public class MarkOrderPaidCommand : IRequest<Unit>
{
    public int OrderId { get; set; }
}
