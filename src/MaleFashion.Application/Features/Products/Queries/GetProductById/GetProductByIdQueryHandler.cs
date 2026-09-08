using MaleFashion.Application.Common.Exceptions;
using MaleFashion.Application.Contracts.Repositories;
using MaleFashion.Application.Contracts.Services;
using MaleFashion.Application.DTOs;
using MaleFashion.Domain.Entities;
using MapsterMapper;
using MediatR;

namespace MaleFashion.Application.Features.Products.Queries.GetProductById;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    private readonly IApplicationUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;
    private readonly IMapper _mapper;

    public GetProductByIdQueryHandler(
        IApplicationUnitOfWork unitOfWork, IFileStorageService fileStorageService, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
        _mapper = mapper;
    }

    public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.Products.GetByIdWithCategoryAsync(request.Id)
            ?? throw new NotFoundException(nameof(Product), request.Id);

        var dto = _mapper.Map<ProductDto>(product);
        dto.ImageUrl = product.ImageKey != null ? _fileStorageService.GetFileUrl(product.ImageKey) : null;
        return dto;
    }
}