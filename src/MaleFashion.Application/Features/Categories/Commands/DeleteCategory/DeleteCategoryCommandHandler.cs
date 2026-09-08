using MaleFashion.Application.Common.Exceptions;
using MaleFashion.Application.Contracts.Repositories;
using MaleFashion.Domain.Entities;
using MediatR;

namespace MaleFashion.Application.Features.Categories.Commands.DeleteCategory;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand>
{
    private readonly IApplicationUnitOfWork _unitOfWork;

    public DeleteCategoryCommandHandler(IApplicationUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(request.Id)
            ?? throw new NotFoundException(nameof(Category), request.Id);

        _unitOfWork.Categories.Delete(category);
        await _unitOfWork.SaveChangesAsync();
    }
}