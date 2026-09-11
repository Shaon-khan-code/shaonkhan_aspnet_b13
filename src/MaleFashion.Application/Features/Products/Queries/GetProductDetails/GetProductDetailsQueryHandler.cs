using MaleFashion.Application.Common.Exceptions;
using MaleFashion.Application.Contracts.Repositories;
using MaleFashion.Application.Contracts.Services;
using MaleFashion.Application.DTOs;
using MaleFashion.Domain.Entities;
using MapsterMapper;
using MediatR;

namespace MaleFashion.Application.Features.Products.Queries.GetProductDetails;

public class GetProductDetailsQueryHandler : IRequestHandler<GetProductDetailsQuery, ProductDetailsDto>
{
    private const int RelatedProductCount = 4;

    private readonly IApplicationUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;
    private readonly IMapper _mapper;

    public GetProductDetailsQueryHandler(
        IApplicationUnitOfWork unitOfWork, IFileStorageService fileStorageService, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
        _mapper = mapper;
    }

    public async Task<ProductDetailsDto> Handle(GetProductDetailsQuery request, CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.Products.GetDetailsAsync(request.Id)
            ?? throw new NotFoundException(nameof(Product), request.Id);

        var relatedProducts = await _unitOfWork.Products.GetRelatedProductsAsync(
            product.CategoryId, product.Id, RelatedProductCount);

        var dto = new ProductDetailsDto
        {
            Id = product.Id,
            Name = product.Name,
            Slug = product.Slug,
            Description = product.Description,
            Sku = product.Sku,
            Price = product.Price,
            DiscountPrice = product.DiscountPrice,
            ImageUrl = product.ImageKey != null ? _fileStorageService.GetFileUrl(product.ImageKey) : null,
            CategoryId = product.CategoryId,
            CategoryName = product.Category.Name,
            IsInStock = product.Inventory != null && product.Inventory.AvailableQuantity > 0,
            AvailableQuantity = product.Inventory?.AvailableQuantity,
            RelatedProducts = _mapper.Map<List<ProductDto>>(relatedProducts)
        };

        return dto;
    }
}