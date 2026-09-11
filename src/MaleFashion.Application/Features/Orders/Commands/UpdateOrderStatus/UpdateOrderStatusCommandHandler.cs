
using MaleFashion.Application.Common.Exceptions;
using MaleFashion.Application.Contracts.Repositories;
using MaleFashion.Domain.Entities;
using MediatR;

namespace MaleFashion.Application.Features.Orders.Commands.UpdateOrderStatus;

public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand>
{
    private readonly IApplicationUnitOfWork _unitOfWork;

    public UpdateOrderStatusCommandHandler(IApplicationUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId)
            ?? throw new NotFoundException(nameof(Order), request.OrderId);

        order.Status = request.Status;
        order.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Orders.Update(order);
        await _unitOfWork.SaveChangesAsync();
    }
}