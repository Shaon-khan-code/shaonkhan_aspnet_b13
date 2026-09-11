using MaleFashion.Application.Common.Models;
using MaleFashion.Application.DTOs;
using MaleFashion.Domain.Enums;
using MediatR;

namespace MaleFashion.Application.Features.Orders.Queries.GetOrderList;

public class GetOrderListQuery : IRequest<PagedResult<OrderListItemDto>>
{
    public OrderStatus? Status { get; set; }
    public PaymentStatus? PaymentStatus { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}