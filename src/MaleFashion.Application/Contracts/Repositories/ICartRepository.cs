using MaleFashion.Domain.Entities;

namespace MaleFashion.Application.Contracts.Repositories;

public interface ICartRepository : IRepository<Cart>
{
    Task<Cart?> GetByOwnerAsync(string? userId, Guid? guestId);

    Task<CartItem?> GetCartItemAsync(int cartItemId);
}