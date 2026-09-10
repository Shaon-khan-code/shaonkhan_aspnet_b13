using MaleFashion.Application.Contracts.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace MaleFashion.Infrastructure.Data.Repositories;

public class ApplicationUnitOfWork : IApplicationUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _currentTransaction;

    public ApplicationUnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        Categories = new CategoryRepository(_context);
        Products = new ProductRepository(_context);
        Inventory = new InventoryRepository(_context);
        Carts = new CartRepository(_context);
        Orders = new OrderRepository(_context);
        Addresses = new AddressRepository(_context);
    }

    public ICategoryRepository Categories { get; }
    public IProductRepository Products { get; }
    public IInventoryRepository Inventory { get; }
    public ICartRepository Carts { get; }
    public IOrderRepository Orders { get; }
    public IAddressRepository Addresses { get; }

    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

    public async Task BeginTransactionAsync()
    {
        _currentTransaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
            await (_currentTransaction?.CommitAsync() ?? Task.CompletedTask);
        }
        finally
        {
            await DisposeCurrentTransactionAsync();
        }
    }

    public async Task RollbackTransactionAsync()
    {
        try
        {
            await (_currentTransaction?.RollbackAsync() ?? Task.CompletedTask);
        }
        finally
        {
            await DisposeCurrentTransactionAsync();
        }
    }

    private async Task DisposeCurrentTransactionAsync()
    {
        if (_currentTransaction != null)
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public void Dispose()
    {
        _currentTransaction?.Dispose();
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}