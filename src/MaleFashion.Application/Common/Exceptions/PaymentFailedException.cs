namespace MaleFashion.Application.Common.Exceptions;

public class PaymentFailedException : Exception
{
    public PaymentFailedException(string message) : base(message)
    {
    }
}