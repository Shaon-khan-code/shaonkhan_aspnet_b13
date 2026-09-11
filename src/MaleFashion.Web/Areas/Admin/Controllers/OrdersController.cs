using FluentValidation;
using MaleFashion.Application.Common.Exceptions;
using MaleFashion.Application.Features.Orders.Commands.UpdateOrderStatus;
using MaleFashion.Application.Features.Orders.Queries.GetOrderByIdAdmin;
using MaleFashion.Application.Features.Orders.Queries.GetOrderList;
using MaleFashion.Domain.Constants;
using MaleFashion.Domain.Enums;
using MaleFashion.Web.Areas.Admin.ViewModels.Orders;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MaleFashion.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = RoleNames.Admin)]
public class OrdersController : Controller
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<IActionResult> Index(OrderStatus? status, PaymentStatus? paymentStatus, int pageNumber = 1)
    {
        var result = await _mediator.Send(new GetOrderListQuery
        {
            Status = status,
            PaymentStatus = paymentStatus,
            PageNumber = pageNumber,
            PageSize = 10
        });

        var model = new OrderListViewModel
        {
            Orders = result,
            Status = status,
            PaymentStatus = paymentStatus
        };

        return View(model);
    }

    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var order = await _mediator.Send(new GetOrderByIdAdminQuery { OrderId = id });
            return View(order);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(UpdateOrderStatusViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Invalid status selected.";
            return RedirectToAction(nameof(Details), new { id = model.OrderId });
        }

        try
        {
            await _mediator.Send(new UpdateOrderStatusCommand
            {
                OrderId = model.OrderId,
                Status = model.Status
            });

            TempData["Success"] = "Order status updated successfully.";
        }
        catch (ValidationException)
        {
            TempData["Error"] = "Invalid status selected.";
        }
        catch (NotFoundException)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Details), new { id = model.OrderId });
    }
}