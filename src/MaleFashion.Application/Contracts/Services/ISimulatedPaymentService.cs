using MaleFashion.Application.Common.Models;

namespace MaleFashion.Application.Contracts.Services;

public class PaymentResult
{
    public bool Success { get; set; }
    public string? TransactionId { get; set; }
    public string? FailureReason { get; set; }
}

public interface ISimulatedPaymentService
{
    Task<PaymentResult> ChargeAsync(SimulatedCardDetails card, decimal amount, CancellationToken cancellationToken = default);
}