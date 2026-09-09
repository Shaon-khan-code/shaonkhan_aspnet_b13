namespace MaleFashion.Application.Contracts.Repositories;

public interface IApplicationUnitOfWork : IDisposable
{
    ICategoryRepository Categories { get; }
    IProductRepository Products { get; }
    IInventoryRepository Inventory { get; }

    Task<int> SaveChangesAsync();
}