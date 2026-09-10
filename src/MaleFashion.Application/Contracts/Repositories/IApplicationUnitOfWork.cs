namespace MaleFashion.Application.Contracts.Repositories;

public interface IApplicationUnitOfWork : IDisposable
{
    ICategoryRepository Categories { get; }
    IProductRepository Products { get; }
    IInventoryRepository Inventory { get; }
    ICartRepository Carts { get; }
    IOrderRepository Orders { get; }
    IAddressRepository Addresses { get; }

    Task<int> SaveChangesAsync();

    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}