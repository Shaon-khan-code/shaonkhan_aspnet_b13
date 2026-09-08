using System.Text.Json;
using MaleFashion.Application.Contracts.Services;
using MaleFashion.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MaleFashion.Infrastructure.Services.Recaptcha;

public class RecaptchaService : IRecaptchaService
{
    private const string VerifyUrl = "https://www.google.com/recaptcha/api/siteverify";

    private readonly HttpClient _httpClient;
    private readonly RecaptchaOptions _options;
    private readonly ILogger<RecaptchaService> _logger;

    public RecaptchaService(HttpClient httpClient, IOptions<RecaptchaOptions> options, ILogger<RecaptchaService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<bool> VerifyAsync(string token, string action, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.SecretKey))
        {
            _logger.LogWarning("reCAPTCHA is not configured (Recaptcha:SecretKey missing). Verification skipped (failing closed).");
            return false;
        }

        if (string.IsNullOrWhiteSpace(token))
        {
            return false;
        }

        var parameters = new Dictionary<string, string>
        {
            ["secret"] = _options.SecretKey,
            ["response"] = token
        };

        try
        {
            using var response = await _httpClient.PostAsync(
                VerifyUrl, new FormUrlEncodedContent(parameters), cancellationToken);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            var success = root.TryGetProperty("success", out var s) && s.GetBoolean();
            var score = root.TryGetProperty("score", out var sc) ? sc.GetDouble() : 0.0;
            var returnedAction = root.TryGetProperty("action", out var a) ? a.GetString() : null;

            return success && score >= _options.MinimumScore && returnedAction == action;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "reCAPTCHA verification request failed.");
            return false;
        }
    }
}