using System.Security.Claims;
using FluentValidation;
using MaleFashion.Application.Common.Exceptions;
using MaleFashion.Application.Common.Models;
using MaleFashion.Application.Features.Cart.Queries.GetCart;
using MaleFashion.Application.Features.Checkout.Commands.PlaceOrder;
using MaleFashion.Application.Features.Checkout.Queries.GetOrderConfirmation;
using MaleFashion.Web.Helpers;
using MaleFashion.Web.ViewModels.Checkout;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DomainPaymentMethod = MaleFashion.Domain.Enums.PaymentMethod;

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
            ClearSensitiveFields(model);
            return View(model);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        try
        {
            var order = await _mediator.Send(BuildCommand(model, userId));

            TempData["PaymentLabel"] = DescribePayment(model);
            return RedirectToAction(nameof(Confirmation), new { id = order.Id });
        }
        catch (ValidationException ex)
        {
            foreach (var error in ex.Errors)
            {
                ModelState.AddModelError(string.Empty, error.ErrorMessage);
            }
        }
        catch (PaymentFailedException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }
        catch (ConflictException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }
        catch (NotFoundException)
        {
            ModelState.AddModelError(string.Empty, "We could not find your cart or address. Please try again.");
        }

        ClearSensitiveFields(model);
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Confirmation(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        try
        {
            var order = await _mediator.Send(new GetOrderConfirmationQuery { OrderId = id, UserId = userId });

            ViewData["PaymentLabel"] = TempData.Peek("PaymentLabel") as string;
            return View(order);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    private async Task<object?> LoadCartAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return await _mediator.Send(new GetCartQuery { UserId = userId, GuestId = null });
    }

    private static PlaceOrderCommand BuildCommand(CheckoutViewModel m, string? userId) => new()
    {
        UserId = userId,
        PaymentMethod = ToDomainMethod(m.PaymentMethod),
        WalletNumber = m.WalletNumber,
        TransactionId = m.TransactionId,
        Card = new SimulatedCardDetails
        {
            CardholderName = m.CardholderName ?? string.Empty,
            CardNumber = m.CardNumber ?? string.Empty,
            ExpiryMonth = m.ExpiryMonth ?? string.Empty,
            ExpiryYear = m.ExpiryYear ?? string.Empty,
            Cvv = m.Cvv ?? string.Empty
        },
        NewShippingAddress = new ShippingAddressInput
        {
            FullName = $"{m.FirstName} {m.LastName}".Trim(),
            Phone = m.Phone,
            Line1 = m.AddressLine1,
            Line2 = m.AddressLine2,
            City = m.City,
            State = m.State,
            PostalCode = m.PostalCode,
            Country = m.Country
        }
    };

    private static DomainPaymentMethod ToDomainMethod(CheckoutPaymentMethod method) => method switch
    {
        CheckoutPaymentMethod.Bkash => DomainPaymentMethod.Bkash,
        CheckoutPaymentMethod.Nagad => DomainPaymentMethod.Nagad,
        CheckoutPaymentMethod.Card => DomainPaymentMethod.Card,
        _ => DomainPaymentMethod.CashOnDelivery
    };

    private static string DescribePayment(CheckoutViewModel m) => m.PaymentMethod switch
    {
        CheckoutPaymentMethod.Bkash => $"bKash (TrxID {m.TransactionId?.Trim().ToUpperInvariant()})",
        CheckoutPaymentMethod.Nagad => $"Nagad (TrxID {m.TransactionId?.Trim().ToUpperInvariant()})",
        CheckoutPaymentMethod.Card => "Card",
        _ => "Cash on Delivery"
    };

    
    private void ClearSensitiveFields(CheckoutViewModel model)
    {
        ModelState.Remove(nameof(CheckoutViewModel.CardNumber));
        ModelState.Remove(nameof(CheckoutViewModel.Cvv));
        model.CardNumber = null;
        model.Cvv = null;
    }
}
