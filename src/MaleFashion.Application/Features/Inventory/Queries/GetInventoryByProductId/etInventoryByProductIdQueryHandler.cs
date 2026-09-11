using MaleFashion.Application.Common.Exceptions;
using MaleFashion.Application.Contracts.Repositories;
using MaleFashion.Application.DTOs;
using MapsterMapper;
using MediatR;

namespace MaleFashion.Application.Features.Inventory.Queries.GetInventoryByProductId;

public class GetInventoryByProductIdQueryHandler : IRequestHandler<GetInventoryByProductIdQuery, InventoryDto>
{
    private readonly IApplicationUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetInventoryByProductIdQueryHandler(IApplicationUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<InventoryDto> Handle(GetInventoryByProductIdQuery request, CancellationToken cancellationToken)
    {
        var inventory = await _unitOfWork.Inventory.GetByProductIdAsync(request.ProductId)
            ?? throw new NotFoundException(nameof(Domain.Entities.Inventory), request.ProductId);

        return _mapper.Map<InventoryDto>(inventory);
    }
}