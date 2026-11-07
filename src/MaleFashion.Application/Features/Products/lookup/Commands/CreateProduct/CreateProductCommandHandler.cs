using MaleFashion.Application.Contracts.Repositories;
using MaleFashion.Application.Contracts.Services;
using MaleFashion.Application.DTOs;
using MaleFashion.Application.Features.Products.Commands.Common;
using MaleFashion.Domain.Entities;
using MapsterMapper;
using MediatR;

namespace MaleFashion.Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IApplicationUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;
    private readonly IMapper _mapper;

    public CreateProductCommandHandler(
        IApplicationUnitOfWork unitOfWork, IFileStorageService fileStorageService, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
        _mapper = mapper;
    }

    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var category = await ProductCommandHelpers.GetCategoryOrThrowAsync(_unitOfWork, request.CategoryId);

        string? imageKey = null;
        if (request.ImageStream != null)
        {
            var extension = Path.GetExtension(request.ImageFileName);
            var uniqueFileName = $"{Guid.NewGuid():N}{extension}";

            imageKey = await _fileStorageService.UploadFileAsync(
                request.ImageStream,
                uniqueFileName,
                request.ImageContentType!,
                "products",
                cancellationToken);
        }

        var product = new Product
        {
            Name = request.Name,
            Slug = request.Slug,
            Description = request.Description,
            Sku = request.Sku,
            Price = request.Price,
            DiscountPrice = request.DiscountPrice,
            IsActive = request.IsActive,
            ImageKey = imageKey,
            CategoryId = category.Id,
            Category = category,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Products.AddAsync(product);

        var inventory = new MaleFashion.Domain.Entities.Inventory
        {
            Product = product,
            QuantityInStock = request.InitialStock,
            ReservedQuantity = 0,
            ReorderLevel = request.ReorderLevel,
            LastUpdatedAt = DateTime.UtcNow
        };
        await _unitOfWork.Inventory.AddAsync(inventory);

        await _unitOfWork.SaveChangesAsync();

        var dto = _mapper.Map<ProductDto>(product);
        dto.ImageUrl = imageKey != null ? _fileStorageService.GetFileUrl(imageKey) : null;
        return dto;
    }
}