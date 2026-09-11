using MaleFashion.Application.Common.Exceptions;
using MaleFashion.Application.Contracts.Repositories;
using MaleFashion.Application.DTOs;
using MapsterMapper;
using MediatR;

namespace MaleFashion.Application.Features.Inventory.Commands.CreateInventory;

public class CreateInventoryCommandHandler : IRequestHandler<CreateInventoryCommand, InventoryDto>
{
    private readonly IApplicationUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateInventoryCommandHandler(IApplicationUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<InventoryDto> Handle(CreateInventoryCommand request, CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(request.ProductId)
            ?? throw new NotFoundException(nameof(Domain.Entities.Product), request.ProductId);

        var existing = await _unitOfWork.Inventory.GetByProductIdAsync(request.ProductId);
        if (existing != null)
        {
            throw new ConflictException(
                $"An inventory record already exists for product \"{product.Name}\". Use update instead.");
        }

        var inventory = new Domain.Entities.Inventory
        {
            ProductId = product.Id,
            Product = product,
            QuantityInStock = request.QuantityInStock,
            ReservedQuantity = 0,
            ReorderLevel = request.ReorderLevel,
            LastUpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Inventory.AddAsync(inventory);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<InventoryDto>(inventory);
    }
}