using MaleFashion.Application.Common.Exceptions;
using MaleFashion.Application.Contracts.Repositories;
using MaleFashion.Domain.Entities;

namespace MaleFashion.Application.Features.Products.Commands.Common;

public static class ProductCommandHelpers
{
    public static async Task<Category> GetCategoryOrThrowAsync(IApplicationUnitOfWork unitOfWork, int categoryId)
    {
        var category = await unitOfWork.Categories.GetByIdAsync(categoryId);
        return category ?? throw new NotFoundException(nameof(Category), categoryId);
    }
}