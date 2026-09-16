
using MaleFashion.Application.Common.Exceptions;
using MaleFashion.Application.Contracts.Repositories;
using MaleFashion.Application.DTOs;
using MaleFashion.Domain.Entities;
using MapsterMapper;
using MediatR;

namespace MaleFashion.Application.Features.Checkout.Queries.GetOrderConfirmation;

public class GetOrderConfirmationQueryHandler : IRequestHandler<GetOrderConfirmationQuery, OrderDto>
{
    private readonly IApplicationUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetOrderConfirmationQueryHandler(IApplicationUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<OrderDto> Handle(GetOrderConfirmationQuery request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Orders.GetByIdWithItemsAsync(request.OrderId)
            ?? throw new NotFoundException(nameof(Order), request.OrderId);

        var isOwner =
            (request.UserId != null && order.UserId == request.UserId) ||
            (request.GuestId.HasValue && order.GuestId == request.GuestId);

        if (!isOwner)
        {
            throw new NotFoundException(nameof(Order), request.OrderId);
        }

        return _mapper.Map<OrderDto>(order);
    }
}