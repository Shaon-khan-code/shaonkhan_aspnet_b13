using MaleFashion.Application.Common.Exceptions;
using MaleFashion.Application.Contracts.Repositories;
using MaleFashion.Domain.Enums;
using MediatR;

namespace MaleFashion.Application.Features.Orders.Commands.MarkOrderPaid;

public class MarkOrderPaidCommandHandler : IRequestHandler<MarkOrderPaidCommand, Unit>
{
    private readonly IApplicationUnitOfWork _unitOfWork;

    public MarkOrderPaidCommandHandler(IApplicationUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(MarkOrderPaidCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId)
            ?? throw new NotFoundException(nameof(MaleFashion.Domain.Entities.Order), request.OrderId);

        if (order.PaymentStatus == PaymentStatus.Paid)
        {
            throw new ConflictException("This order is already marked as paid.");
        }

        if (order.Status == OrderStatus.Cancelled)
        {
            throw new ConflictException("A cancelled order cannot be marked as paid.");
        }

        order.PaymentStatus = PaymentStatus.Paid;

        
        if (order.Status == OrderStatus.Pending)
        {
            order.Status = OrderStatus.Processing;
        }

        await _unitOfWork.SaveChangesAsync();
        return Unit.Value;
    }
}
