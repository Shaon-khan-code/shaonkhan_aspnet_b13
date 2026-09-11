using MaleFashion.Application.DTOs;
using MediatR;

namespace MaleFashion.Application.Features.Inventory.Commands.CreateInventory;

public class CreateInventoryCommand : IRequest<InventoryDto>
{
    public int ProductId { get; set; }
    public int QuantityInStock { get; set; }
    public int ReorderLevel { get; set; }
}