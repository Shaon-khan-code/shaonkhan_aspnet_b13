using MaleFashion.Application.Common.Models;
using MaleFashion.Application.DTOs;
using MediatR;

namespace MaleFashion.Application.Features.Inventory.Queries.GetInventoryList;

public class GetInventoryListQuery : IRequest<PagedResult<InventoryDto>>
{
    public bool? LowStockOnly { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}