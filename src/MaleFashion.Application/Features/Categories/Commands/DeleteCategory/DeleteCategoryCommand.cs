using MediatR;

namespace MaleFashion.Application.Features.Categories.Commands.DeleteCategory;

public class DeleteCategoryCommand : IRequest
{
    public int Id { get; set; }
}