using System.Text.RegularExpressions;

namespace MaleFashion.Application.Common.Helpers;

public static class CardValidationHelper
{
    public static bool IsValidCardNumber(string? cardNumber)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
        {
            return false;
        }

        var digitsOnly = Regex.Replace(cardNumber, @"[\s-]", "");

        if (!Regex.IsMatch(digitsOnly, @"^\d{13,19}$"))
        {
            return false;
        }

        return PassesLuhnCheck(digitsOnly);
    }

    public static bool IsValidExpiry(string? month, string? year, DateTime utcNow)
    {
        if (!Regex.IsMatch(month ?? "", @"^(0[1-9]|1[0-2])$"))
        {
            return false;
        }

        if (!Regex.IsMatch(year ?? "", @"^\d{4}$"))
        {
            return false;
        }

        var expiryMonth = int.Parse(month!);
        var expiryYear = int.Parse(year!);

        // A card is valid through the LAST day of its expiry month.
        var expiryDate = new DateTime(expiryYear, expiryMonth, 1).AddMonths(1).AddDays(-1);

        return expiryDate >= utcNow.Date;
    }

    public static bool IsValidCvv(string? cvv)
    {
        return !string.IsNullOrWhiteSpace(cvv) && Regex.IsMatch(cvv, @"^\d{3,4}$");
    }

    private static bool PassesLuhnCheck(string digitsOnly)
    {
        var sum = 0;
        var alternate = false;

        for (var i = digitsOnly.Length - 1; i >= 0; i--)
        {
            var digit = digitsOnly[i] - '0';

            if (alternate)
            {
                digit *= 2;
                if (digit > 9)
                {
                    digit -= 9;
                }
            }

            sum += digit;
            alternate = !alternate;
        }

        return sum % 10 == 0;
    }
}