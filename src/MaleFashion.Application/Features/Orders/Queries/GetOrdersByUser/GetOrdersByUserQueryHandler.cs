using MaleFashion.Application.Contracts.Repositories;
using MaleFashion.Application.DTOs;
using MapsterMapper;
using MediatR;

namespace MaleFashion.Application.Features.Orders.Queries.GetOrdersByUser;

public class GetOrdersByUserQueryHandler : IRequestHandler<GetOrdersByUserQuery, List<OrderSummaryDto>>
{
    private readonly IApplicationUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetOrdersByUserQueryHandler(IApplicationUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<OrderSummaryDto>> Handle(GetOrdersByUserQuery request, CancellationToken cancellationToken)
    {
        var orders = await _unitOfWork.Orders.GetByUserIdAsync(request.UserId);
        return _mapper.Map<List<OrderSummaryDto>>(orders);
    }
}