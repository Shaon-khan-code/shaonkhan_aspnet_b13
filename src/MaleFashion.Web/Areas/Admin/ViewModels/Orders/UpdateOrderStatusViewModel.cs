using System.ComponentModel.DataAnnotations;
using MaleFashion.Domain.Enums;

namespace MaleFashion.Web.Areas.Admin.ViewModels.Orders;

public class UpdateOrderStatusViewModel
{
    public int OrderId { get; set; }

    [Required]
    [Display(Name = "Order Status")]
    public OrderStatus Status { get; set; }
}