using System.Security.Claims;
using MaleFashion.Application.Features.Cart.Queries.GetCart;
using MaleFashion.Web.Helpers;
using MaleFashion.Web.ViewModels.Checkout;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MaleFashion.Web.Controllers;

[Authorize]
public class CheckoutController : Controller
{
    private readonly IMediator _mediator;

    public CheckoutController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var cart = await LoadCartAsync();
        if (!cart.ToCartLines().Any())
        {
            TempData["CartError"] = "Your cart is empty.";
            return RedirectToAction("Index", "Cart");
        }

        return View(new CheckoutViewModel
        {
            Email = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty,
            Cart = cart
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(CheckoutViewModel model)
    {
        var cart = await LoadCartAsync();
        if (!cart.ToCartLines().Any())
        {
            TempData["CartError"] = "Your cart is empty.";
            return RedirectToAction("Index", "Cart");
        }

        model.Cart = cart;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        
        ModelState.AddModelError(string.Empty, "Order placement is not connected yet.");
        return View(model);
    }

    private async Task<object?> LoadCartAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return await _mediator.Send(new GetCartQuery { UserId = userId, GuestId = null });
    }
}
