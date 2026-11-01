using MaleFashion.Application.Common.Models;
using MaleFashion.Application.Contracts.Repositories;
using MaleFashion.Application.Contracts.Services;
using MaleFashion.Application.DTOs;
using MapsterMapper;
using MediatR;

namespace MaleFashion.Application.Features.Products.Queries.GetProductList;

public class GetProductListQueryHandler
    : IRequestHandler<GetProductListQuery, PagedResult<ProductDto>>
{
    private readonly IApplicationUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IFileStorageService _fileStorageService;

    public GetProductListQueryHandler(
        IApplicationUnitOfWork unitOfWork,
        IMapper mapper,
        IFileStorageService fileStorageService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _fileStorageService = fileStorageService;
    }

    public async Task<PagedResult<ProductDto>> Handle(
        GetProductListQuery request,
        CancellationToken cancellationToken)
    {
        var (items, totalCount) =
            await _unitOfWork.Products.GetPagedAsync(
                request.SearchTerm,
                request.CategoryId,
                request.IsActive,
                request.MinPrice,
                request.MaxPrice,
                request.SortBy,
                request.PageNumber,
                request.PageSize);

        var productDtos = _mapper.Map<List<ProductDto>>(items);

        for (var i = 0; i < items.Count; i++)
        {
            var imageKey = items[i].ImageKey?.Trim();

            productDtos[i].ImageUrl = ResolveImageUrl(imageKey);
        }

        return new PagedResult<ProductDto>
        {
            Items = productDtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    private string? ResolveImageUrl(string? imageKey)
    {
        if (string.IsNullOrWhiteSpace(imageKey))
            return null;

        // Full URL: return it unchanged.
        if (Uri.TryCreate(
                imageKey,
                UriKind.Absolute,
                out var uri) &&
            (uri.Scheme == Uri.UriSchemeHttp ||
             uri.Scheme == Uri.UriSchemeHttps))
        {
            return imageKey;
        }

        // Legacy local image paths.
        if (imageKey.StartsWith("~/"))
            return imageKey;

        if (imageKey.StartsWith("/"))
            return imageKey;

        if (imageKey.StartsWith(
                "img/product/",
                StringComparison.OrdinalIgnoreCase))
        {
            return "/storefront/" + imageKey;
        }

        if (imageKey.StartsWith(
                "storefront/",
                StringComparison.OrdinalIgnoreCase))
        {
            return "/" + imageKey;
        }

        // Other keys are treated as S3/CloudFront keys.
        return _fileStorageService.GetFileUrl(imageKey);
    }
}