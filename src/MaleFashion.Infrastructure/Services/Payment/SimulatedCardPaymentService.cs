using MaleFashion.Application.Common.Helpers;
using MaleFashion.Application.Common.Models;
using MaleFashion.Application.Contracts.Services;

namespace MaleFashion.Infrastructure.Services.Payment;

public class SimulatedCardPaymentService : ISimulatedPaymentService
{
    public Task<PaymentResult> ChargeAsync(SimulatedCardDetails card, decimal amount, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(card.CardholderName))
        {
            return Task.FromResult(Failure("Cardholder name is required."));
        }

        if (!CardValidationHelper.IsValidCardNumber(card.CardNumber))
        {
            return Task.FromResult(Failure("The card number is invalid."));
        }

        if (!CardValidationHelper.IsValidExpiry(card.ExpiryMonth, card.ExpiryYear, DateTime.UtcNow))
        {
            return Task.FromResult(Failure("The card has expired or the expiry date is invalid."));
        }

        if (!CardValidationHelper.IsValidCvv(card.Cvv))
        {
            return Task.FromResult(Failure("The CVV is invalid."));
        }

        if (amount <= 0)
        {
            return Task.FromResult(Failure("Invalid charge amount."));
        }

        // Simulation-only test hook: a card number ending in "0000" deliberately
        // simulates a gateway decline (e.g. insufficient funds), even though it
        // is otherwise a well-formed, unexpired, Luhn-valid card. This exists so
        // both the "invalid card" and "valid card but declined" paths can be
        // tested without a real payment gateway.
        var digitsOnly = card.CardNumber.Replace(" ", "").Replace("-", "");
        if (digitsOnly.EndsWith("0000"))
        {
            return Task.FromResult(Failure("The card was declined by the issuing bank."));
        }

        // NOTE: card.CardNumber and card.Cvv are used only in-memory, for this
        // validation/simulation, and are never persisted, logged, or passed to
        // any repository/DbContext. Only the resulting PaymentResult
        // (Success/TransactionId/FailureReason) leaves this method.
        return Task.FromResult(new PaymentResult
        {
            Success = true,
            TransactionId = $"SIM-{Guid.NewGuid():N}"
        });
    }

    private static PaymentResult Failure(string reason) => new()
    {
        Success = false,
        FailureReason = reason
    };
}