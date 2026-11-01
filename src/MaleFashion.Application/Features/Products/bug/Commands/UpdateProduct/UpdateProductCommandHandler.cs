
using MaleFashion.Application.Common.Exceptions;
using MaleFashion.Application.Contracts.Repositories;
using MaleFashion.Application.Contracts.Services;
using MaleFashion.Application.DTOs;
using MaleFashion.Application.Features.Products.Commands.Common;
using MaleFashion.Domain.Entities;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MaleFashion.Application.Features.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler
    : IRequestHandler<UpdateProductCommand, ProductDto>
{
    private readonly IApplicationUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateProductCommandHandler> _logger;

    public UpdateProductCommandHandler(
        IApplicationUnitOfWork unitOfWork,
        IFileStorageService fileStorageService,
        IMapper mapper,
        ILogger<UpdateProductCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ProductDto> Handle(
        UpdateProductCommand request,
        CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(request.Id)
            ?? throw new NotFoundException(nameof(Product), request.Id);

        var category =
            await ProductCommandHelpers.GetCategoryOrThrowAsync(
                _unitOfWork, request.CategoryId);

        var oldImageKey = product.ImageKey;
        var imageChanged = false;

        if (request.ImageStream != null)
        {
            var extension = Path.GetExtension(request.ImageFileName);

            var uniqueFileName = $"{Guid.NewGuid():N}{extension}";

            var newImageKey = await _fileStorageService.UploadFileAsync(
                request.ImageStream,
                uniqueFileName,
                request.ImageContentType!,
                "products",
                cancellationToken);

            product.ImageKey = newImageKey;
            imageChanged = true;
        }

        product.Name = request.Name;
        product.Slug = request.Slug;
        product.Description = request.Description;
        product.Sku = request.Sku;
        product.Price = request.Price;
        product.DiscountPrice = request.DiscountPrice;
        product.IsActive = request.IsActive;
        product.CategoryId = category.Id;
        product.Category = category;
        product.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Products.Update(product);
        await _unitOfWork.SaveChangesAsync();

        if (imageChanged &&
            !string.IsNullOrWhiteSpace(oldImageKey) &&
            oldImageKey != product.ImageKey)
        {
            try
            {
                await _fileStorageService.DeleteFileAsync(
                    oldImageKey, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "Failed to delete old product image. Key: {Key}",
                    oldImageKey);
            }
        }

        var dto = _mapper.Map<ProductDto>(product);

        dto.ImageUrl = !string.IsNullOrWhiteSpace(product.ImageKey)
            ? _fileStorageService.GetFileUrl(product.ImageKey)
            : null;

        return dto;
    }
}