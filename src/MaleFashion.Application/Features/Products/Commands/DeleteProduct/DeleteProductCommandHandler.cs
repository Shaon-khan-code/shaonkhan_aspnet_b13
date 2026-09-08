using MaleFashion.Application.Common.Exceptions;
using MaleFashion.Application.Contracts.Repositories;
using MaleFashion.Application.Contracts.Services;
using MaleFashion.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MaleFashion.Application.Features.Products.Commands.DeleteProduct;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
{
    private readonly IApplicationUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;
    private readonly ILogger<DeleteProductCommandHandler> _logger;

    public DeleteProductCommandHandler(
        IApplicationUnitOfWork unitOfWork, IFileStorageService fileStorageService, ILogger<DeleteProductCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
        _logger = logger;
    }

    public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(request.Id)
            ?? throw new NotFoundException(nameof(Product), request.Id);

        var hasOrders = await _unitOfWork.Products.HasOrderItemsAsync(request.Id);
        if (hasOrders)
        {
            throw new ConflictException(
                "This product cannot be deleted because it has existing orders associated with it. Consider marking it Inactive instead.");
        }

        var imageKey = product.ImageKey;

        _unitOfWork.Products.Delete(product);
        await _unitOfWork.SaveChangesAsync();

        if (!string.IsNullOrEmpty(imageKey))
        {
            try
            {
                await _fileStorageService.DeleteFileAsync(imageKey, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to delete product image after product deletion. Key: {Key}", imageKey);
            }
        }
    }
}