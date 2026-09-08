using MaleFashion.Application.DTOs;
using MediatR;

namespace MaleFashion.Application.Features.Categories.Queries.GetCategoryById;

public class GetCategoryByIdQuery : IRequest<CategoryDto>
{
    public int Id { get; set; }
}