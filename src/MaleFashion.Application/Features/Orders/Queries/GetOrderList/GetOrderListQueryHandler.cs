using MaleFashion.Application.Common.Models;
using MaleFashion.Application.Contracts.Repositories;
using MaleFashion.Application.DTOs;
using MapsterMapper;
using MediatR;

namespace MaleFashion.Application.Features.Orders.Queries.GetOrderList;

public class GetOrderListQueryHandler : IRequestHandler<GetOrderListQuery, PagedResult<OrderListItemDto>>
{
    private readonly IApplicationUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetOrderListQueryHandler(IApplicationUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<OrderListItemDto>> Handle(GetOrderListQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _unitOfWork.Orders.GetPagedAsync(
            request.Status, request.PaymentStatus, request.PageNumber, request.PageSize);

        return new PagedResult<OrderListItemDto>
        {
            Items = _mapper.Map<List<OrderListItemDto>>(items),
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}