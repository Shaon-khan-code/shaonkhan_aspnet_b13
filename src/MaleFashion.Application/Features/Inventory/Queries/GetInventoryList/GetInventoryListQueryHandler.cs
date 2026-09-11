using MaleFashion.Application.Common.Models;
using MaleFashion.Application.Contracts.Repositories;
using MaleFashion.Application.DTOs;
using MapsterMapper;
using MediatR;

namespace MaleFashion.Application.Features.Inventory.Queries.GetInventoryList;

public class GetInventoryListQueryHandler : IRequestHandler<GetInventoryListQuery, PagedResult<InventoryDto>>
{
    private readonly IApplicationUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetInventoryListQueryHandler(IApplicationUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<InventoryDto>> Handle(GetInventoryListQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _unitOfWork.Inventory.GetPagedAsync(
            request.LowStockOnly, request.PageNumber, request.PageSize);

        return new PagedResult<InventoryDto>
        {
            Items = _mapper.Map<List<InventoryDto>>(items),
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}