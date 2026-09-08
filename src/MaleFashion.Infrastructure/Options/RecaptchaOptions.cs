namespace MaleFashion.Infrastructure.Options;

public class RecaptchaOptions
{
    public string SiteKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public double MinimumScore { get; set; } = 0.5;
}