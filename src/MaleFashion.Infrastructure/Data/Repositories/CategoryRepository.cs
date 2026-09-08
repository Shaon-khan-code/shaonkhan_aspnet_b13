// CategoryRepository.cs
using MaleFashion.Application.Contracts.Repositories;
using MaleFashion.Domain.Entities;

namespace MaleFashion.Infrastructure.Data.Repositories;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(ApplicationDbContext context) : base(context) { }
}