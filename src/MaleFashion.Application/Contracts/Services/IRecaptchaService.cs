namespace MaleFashion.Application.Contracts.Services;

public interface IRecaptchaService
{
    Task<bool> VerifyAsync(string token, string action, CancellationToken cancellationToken = default);
}