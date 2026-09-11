using MaleFashion.Application.Common.Models;
using MaleFashion.Application.DTOs;
using MaleFashion.Domain.Enums;

namespace MaleFashion.Web.Areas.Admin.ViewModels.Orders;

public class OrderListViewModel
{
    public PagedResult<OrderListItemDto> Orders { get; set; } = new();
    public OrderStatus? Status { get; set; }
    public PaymentStatus? PaymentStatus { get; set; }
}