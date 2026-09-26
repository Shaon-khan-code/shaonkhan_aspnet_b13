using MaleFashion.Application.DTOs;
using MediatR;

namespace MaleFashion.Application.Features.Inventory.Queries.GetLowStockProducts;

public class GetLowStockProductsQuery : IRequest<List<InventoryDto>>
{
}