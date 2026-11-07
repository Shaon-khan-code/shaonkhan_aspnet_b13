using FluentValidation;
using MaleFashion.Application.Common.Exceptions;
using MaleFashion.Application.Features.Cart.Commands.AddToCart;
using MaleFashion.Application.Features.Cart.Commands.RemoveCartItem;
using MaleFashion.Application.Features.Cart.Commands.UpdateCartItemQuantity;
using MaleFashion.Application.Features.Cart.Queries.GetCart;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MaleFashion.Web.Controllers;

public class CartController : Controller
{
    private const string GuestCartCookieName = "MaleFashion.GuestCartId";
    private readonly IMediator _mediator;

    public CartController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<IActionResult> Index()
    {
        var (userId, guestId) = GetOwnerIdentity();
        var cart = await _mediator.Send(new GetCartQuery { UserId = userId, GuestId = guestId });
        return View(cart);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int productId, int quantity = 1)
    {
        // Buy Now স্ক্রিপ্ট (fetch) এই header পাঠায়, তখন JSON ফেরত দেওয়া হয়
        var isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
        var (userId, guestId) = GetOrCreateOwnerIdentity();
        string? error = null;

        try
        {
            await _mediator.Send(new AddToCartCommand
            {
                UserId = userId,
                GuestId = guestId,
                ProductId = productId,
                Quantity = quantity
            });
        }
        catch (ValidationException ex)
        {
            error = string.Join(" ", ex.Errors.Select(e => e.ErrorMessage));
        }
        catch (ConflictException ex)
        {
            error = ex.Message;
        }
        catch (NotFoundException)
        {
            error = "Product not found.";
        }

        if (isAjax)
        {
            return error == null
                ? Ok(new { success = true })
                : BadRequest(new { success = false, message = error });
        }

        if (error == null)
        {
            TempData["CartSuccess"] = "Item added to cart.";
        }
        else
        {
            TempData["CartError"] = error;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity)
    {
        var (userId, guestId) = GetOwnerIdentity();

        try
        {
            await _mediator.Send(new UpdateCartItemQuantityCommand
            {
                UserId = userId,
                GuestId = guestId,
                CartItemId = cartItemId,
                Quantity = quantity
            });
        }
        catch (ValidationException ex)
        {
            TempData["CartError"] = string.Join(" ", ex.Errors.Select(e => e.ErrorMessage));
        }
        catch (ConflictException ex)
        {
            TempData["CartError"] = ex.Message;
        }
        catch (NotFoundException)
        {
            TempData["CartError"] = "Cart item not found.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(int cartItemId)
    {
        var (userId, guestId) = GetOwnerIdentity();

        try
        {
            await _mediator.Send(new RemoveCartItemCommand
            {
                UserId = userId,
                GuestId = guestId,
                CartItemId = cartItemId
            });

            TempData["CartSuccess"] = "Item removed.";
        }
        catch (NotFoundException)
        {
            TempData["CartError"] = "Cart item not found.";
        }

        return RedirectToAction(nameof(Index));
    }

    private (string? UserId, Guid? GuestId) GetOwnerIdentity()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return (User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, null);
        }

        if (Guid.TryParse(Request.Cookies[GuestCartCookieName], out var guestId))
        {
            return (null, guestId);
        }

        return (null, null);
    }

    private (string? UserId, Guid? GuestId) GetOrCreateOwnerIdentity()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return (User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, null);
        }

        if (Guid.TryParse(Request.Cookies[GuestCartCookieName], out var existingGuestId))
        {
            return (null, existingGuestId);
        }

        var newGuestId = Guid.NewGuid();
        Response.Cookies.Append(GuestCartCookieName, newGuestId.ToString(), new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddDays(30)
        });

        return (null, newGuestId);
    }
}