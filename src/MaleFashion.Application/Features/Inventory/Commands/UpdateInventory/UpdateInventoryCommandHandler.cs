using MaleFashion.Application.Common.Exceptions;
using MaleFashion.Application.Contracts.Repositories;
using MaleFashion.Application.DTOs;
using MapsterMapper;
using MediatR;

namespace MaleFashion.Application.Features.Inventory.Commands.UpdateInventory;

public class UpdateInventoryCommandHandler : IRequestHandler<UpdateInventoryCommand, InventoryDto>
{
    private readonly IApplicationUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateInventoryCommandHandler(IApplicationUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<InventoryDto> Handle(UpdateInventoryCommand request, CancellationToken cancellationToken)
    {
        var inventory = await _unitOfWork.Inventory.GetByIdAsync(request.Id)
            ?? throw new NotFoundException(nameof(Domain.Entities.Inventory), request.Id);

        inventory.QuantityInStock = request.QuantityInStock;
        inventory.ReorderLevel = request.ReorderLevel;
        inventory.LastUpdatedAt = DateTime.UtcNow;

        _unitOfWork.Inventory.Update(inventory);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<InventoryDto>(inventory);
    }
}