using MaleFashion.Application.DTOs;
using MediatR;

namespace MaleFashion.Application.Features.Categories.Queries.GetCategoryList;

public class GetCategoryListQuery : IRequest<List<CategoryDto>>
{
}


