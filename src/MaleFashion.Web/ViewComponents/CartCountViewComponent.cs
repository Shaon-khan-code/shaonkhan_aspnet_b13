using System.Security.Claims;
using MaleFashion.Application.Features.Cart.Queries.GetCart;
using MaleFashion.Web.Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MaleFashion.Web.ViewComponents;


public class CartCountViewComponent : ViewComponent
{
    private const string GuestCartCookieName = "MaleFashion.GuestCartId";

    private readonly IMediator _mediator;
    private readonly ILogger<CartCountViewComponent> _logger;

    public CartCountViewComponent(IMediator mediator, ILogger<CartCountViewComponent> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        string? userId = null;
        Guid? guestId = null;

        if (UserClaimsPrincipal.Identity?.IsAuthenticated == true)
        {
            userId = UserClaimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
        }
        else if (Guid.TryParse(Request.Cookies[GuestCartCookieName], out var parsed))
        {
            guestId = parsed;
        }
        else
        {
            return View(0);
        }

        try
        {
            var cart = await _mediator.Send(new GetCartQuery { UserId = userId, GuestId = guestId });
            return View(cart.ToCartLines().Sum(l => l.Quantity));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not load the cart count for the header.");
            return View(0);
        }
    }
}
