using MaleFashion.Application.Contracts.Repositories;
using MaleFashion.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MaleFashion.Infrastructure.Data.Repositories;

public class CartRepository : Repository<Cart>, ICartRepository
{
    public CartRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Cart?> GetByOwnerAsync(string? userId, Guid? guestId)
    {
        var query = DbSet
            .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                    .ThenInclude(p => p.Inventory);

        if (userId != null)
        {
            return await query.FirstOrDefaultAsync(c => c.UserId == userId);
        }

        if (guestId.HasValue)
        {
            return await query.FirstOrDefaultAsync(c => c.GuestId == guestId.Value);
        }

        return null;
    }

    public async Task<CartItem?> GetCartItemAsync(int cartItemId)
    {
        return await Context.Set<CartItem>()
            .Include(ci => ci.Cart)
            .Include(ci => ci.Product)
                .ThenInclude(p => p.Inventory)
            .FirstOrDefaultAsync(ci => ci.Id == cartItemId);
    }
}