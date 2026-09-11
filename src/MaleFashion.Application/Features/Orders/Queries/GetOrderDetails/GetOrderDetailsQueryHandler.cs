using MaleFashion.Application.Common.Exceptions;
using MaleFashion.Application.Contracts.Repositories;
using MaleFashion.Application.DTOs;
using MaleFashion.Domain.Entities;
using MapsterMapper;
using MediatR;

namespace MaleFashion.Application.Features.Orders.Queries.GetOrderDetails;

public class GetOrderDetailsQueryHandler : IRequestHandler<GetOrderDetailsQuery, OrderDto>
{
    private readonly IApplicationUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetOrderDetailsQueryHandler(IApplicationUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<OrderDto> Handle(GetOrderDetailsQuery request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Orders.GetByIdWithItemsAsync(request.OrderId)
            ?? throw new NotFoundException(nameof(Order), request.OrderId);

        // Ownership check: a customer must only ever see their own order.
        // Returning NotFoundException (not a distinct "Forbidden") avoids
        // confirming to an attacker that a given OrderId exists at all —
        // same information-hiding principle already used for Cart item
        // ownership checks and the Login error message.
        if (order.UserId != request.UserId)
        {
            throw new NotFoundException(nameof(Order), request.OrderId);
        }

        return _mapper.Map<OrderDto>(order);
    }
}