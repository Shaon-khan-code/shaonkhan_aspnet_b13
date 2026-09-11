using MaleFashion.Application.DTOs;
using MediatR;

namespace MaleFashion.Application.Features.Inventory.Queries.GetInventoryByProductId;

public class GetInventoryByProductIdQuery : IRequest<InventoryDto>
{
    public int ProductId { get; set; }
}