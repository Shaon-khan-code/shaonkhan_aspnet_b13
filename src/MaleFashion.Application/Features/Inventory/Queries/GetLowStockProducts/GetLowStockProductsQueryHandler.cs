using MaleFashion.Application.Contracts.Repositories;
using MaleFashion.Application.DTOs;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MaleFashion.Application.Features.Inventory.Queries.GetLowStockProducts;

public class GetLowStockProductsQueryHandler : IRequestHandler<GetLowStockProductsQuery, List<InventoryDto>>
{
    private readonly IApplicationUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetLowStockProductsQueryHandler> _logger;

    public GetLowStockProductsQueryHandler(
        IApplicationUnitOfWork unitOfWork, IMapper mapper, ILogger<GetLowStockProductsQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<InventoryDto>> Handle(GetLowStockProductsQuery request, CancellationToken cancellationToken)
    {
        var lowStockItems = await _unitOfWork.Inventory.GetAllLowStockAsync();

        foreach (var item in lowStockItems)
        {
            _logger.LogWarning(
                "Low stock detected: {ProductName} (SKU: {Sku}) — Available: {Available}, Reorder level: {ReorderLevel}",
                item.Product.Name, item.Product.Sku, item.QuantityInStock - item.ReservedQuantity, item.ReorderLevel);
        }

        return _mapper.Map<List<InventoryDto>>(lowStockItems);
    }
}