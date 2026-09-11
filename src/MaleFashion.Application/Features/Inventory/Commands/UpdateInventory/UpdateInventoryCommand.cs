using MaleFashion.Application.DTOs;
using MediatR;

namespace MaleFashion.Application.Features.Inventory.Commands.UpdateInventory;

public class UpdateInventoryCommand : IRequest<InventoryDto>
{
    public int Id { get; set; }
    public int QuantityInStock { get; set; }
    public int ReorderLevel { get; set; }
}