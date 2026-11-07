using MaleFashion.Application.Contracts.Repositories;
using MaleFashion.Domain.Entities;
using MaleFashion.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace MaleFashion.Infrastructure.Data.Repositories;

public class OrderRepository : Repository<Order>, IOrderRepository
{
    public OrderRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Order?> GetByIdWithItemsAsync(int id)
    {
        return await DbSet
            .Include(o => o.OrderItems)
            .Include(o => o.ShippingAddress)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<List<Order>> GetByUserIdAsync(string userId)
    {
        return await DbSet
            .AsNoTracking()
            .Include(o => o.OrderItems)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }
    public async Task<(List<Order> Items, int TotalCount)> GetPagedAsync(
    OrderStatus? status,
    PaymentStatus? paymentStatus,
    int pageNumber,
    int pageSize)
    {
        var query = DbSet.AsNoTracking().Include(o => o.ShippingAddress).AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(o => o.Status == status.Value);
        }

        if (paymentStatus.HasValue)
        {
            query = query.Where(o => o.PaymentStatus == paymentStatus.Value);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
    public async Task<decimal> GetTotalRevenueAsync(PaymentStatus paymentStatus)
    {
        return await DbSet
            .Where(o => o.PaymentStatus == paymentStatus)
            .SumAsync(o => o.TotalAmount);
    }

    public async Task<List<Order>> GetRecentOrdersAsync(int count)
    {
        return await DbSet
            .AsNoTracking()
            .Include(o => o.ShippingAddress)
            .OrderByDescending(o => o.CreatedAt)
            .Take(count)
            .ToListAsync();
    }
    public async Task<bool> PaymentReferenceExistsAsync(string paymentReference)
    {
        return await DbSet.AnyAsync(o => o.PaymentReference == paymentReference);
    }

}