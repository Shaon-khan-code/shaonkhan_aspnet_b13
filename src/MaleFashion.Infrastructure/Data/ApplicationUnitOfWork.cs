using MaleFashion.Application.Contracts.Repositories;

namespace MaleFashion.Infrastructure.Data.Repositories;

public class ApplicationUnitOfWork : IApplicationUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public ApplicationUnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        Categories = new CategoryRepository(_context);
        Products = new ProductRepository(_context);
    }

    public ICategoryRepository Categories { get; }
    public IProductRepository Products { get; }

    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}