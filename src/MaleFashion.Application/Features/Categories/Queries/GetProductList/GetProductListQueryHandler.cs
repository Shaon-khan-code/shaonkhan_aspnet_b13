using MaleFashion.Application.Common.Models;
using MaleFashion.Application.Contracts.Repositories;
using MaleFashion.Application.DTOs;
using MapsterMapper;
using MediatR;

namespace MaleFashion.Application.Features.Products.Queries.GetProductList;

public class GetProductListQueryHandler : IRequestHandler<GetProductListQuery, PagedResult<ProductDto>>
{
    private readonly IApplicationUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetProductListQueryHandler(IApplicationUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<ProductDto>> Handle(GetProductListQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _unitOfWork.Products.GetPagedAsync(
            request.SearchTerm,
            request.CategoryId,
            request.IsActive,
            request.PageNumber,
            request.PageSize);

        return new PagedResult<ProductDto>
        {
            Items = _mapper.Map<List<ProductDto>>(items),
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}