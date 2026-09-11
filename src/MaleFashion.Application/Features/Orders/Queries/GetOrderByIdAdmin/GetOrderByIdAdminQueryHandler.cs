using MaleFashion.Application.Common.Exceptions;
using MaleFashion.Application.Contracts.Repositories;
using MaleFashion.Application.DTOs;
using MaleFashion.Domain.Entities;
using MapsterMapper;
using MediatR;

namespace MaleFashion.Application.Features.Orders.Queries.GetOrderByIdAdmin;

public class GetOrderByIdAdminQueryHandler : IRequestHandler<GetOrderByIdAdminQuery, OrderDto>
{
    private readonly IApplicationUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetOrderByIdAdminQueryHandler(IApplicationUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<OrderDto> Handle(GetOrderByIdAdminQuery request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Orders.GetByIdWithItemsAsync(request.OrderId)
            ?? throw new NotFoundException(nameof(Order), request.OrderId);

        return _mapper.Map<OrderDto>(order);
    }
}