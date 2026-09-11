using MaleFashion.Application.Common.Exceptions;
using MaleFashion.Application.Features.Orders.Queries.GetOrderDetails;
using MaleFashion.Application.Features.Orders.Queries.GetOrdersByUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MaleFashion.Web.Controllers;

[Authorize]
public class OrdersController : Controller
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var orders = await _mediator.Send(new GetOrdersByUserQuery { UserId = userId });
        return View(orders);
    }

    public async Task<IActionResult> Details(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        try
        {
            var order = await _mediator.Send(new GetOrderDetailsQuery { OrderId = id, UserId = userId });
            return View(order);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }
}