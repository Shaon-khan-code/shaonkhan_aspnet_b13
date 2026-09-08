namespace MaleFashion.Application.Contracts.Repositories;

public interface IApplicationUnitOfWork : IDisposable
{
    ICategoryRepository Categories { get; }
    IProductRepository Products { get; }

    Task<int> SaveChangesAsync();
}