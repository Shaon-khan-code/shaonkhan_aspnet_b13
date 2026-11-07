using MaleFashion.Domain.Entities;
using MaleFashion.Domain.Enums;

namespace MaleFashion.Application.Contracts.Repositories;

public interface IOrderRepository : IRepository<Order>
{
    Task<Order?> GetByIdWithItemsAsync(int id);

    Task<List<Order>> GetByUserIdAsync(string userId);

    Task<(List<Order> Items, int TotalCount)> GetPagedAsync(
        OrderStatus? status,
        PaymentStatus? paymentStatus,
        int pageNumber,
        int pageSize);

    Task<decimal> GetTotalRevenueAsync(PaymentStatus paymentStatus);

    Task<List<Order>> GetRecentOrdersAsync(int count);
    Task<bool> PaymentReferenceExistsAsync(string paymentReference);
}