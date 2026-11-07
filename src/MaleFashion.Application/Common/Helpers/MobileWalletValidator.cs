using System.Text.RegularExpressions;

namespace MaleFashion.Application.Common.Helpers;


public static class MobileWalletValidator
{
    private static readonly Regex WalletRegex = new(@"^(?:\+?88)?(01[3-9]\d{8})$", RegexOptions.Compiled);
    private static readonly Regex TransactionRegex = new("^[A-Za-z0-9]{8,12}$", RegexOptions.Compiled);

   
    public static string? Normalize(string? walletNumber)
    {
        if (string.IsNullOrWhiteSpace(walletNumber))
        {
            return null;
        }

        var cleaned = walletNumber.Replace(" ", string.Empty).Replace("-", string.Empty);
        var match = WalletRegex.Match(cleaned);

        return match.Success ? match.Groups[1].Value : null;
    }

    public static bool IsValidWalletNumber(string? walletNumber) => Normalize(walletNumber) != null;

    public static bool IsValidTransactionId(string? transactionId)
        => !string.IsNullOrWhiteSpace(transactionId) && TransactionRegex.IsMatch(transactionId.Trim());

    
    public static string? Validate(string? walletNumber, string? transactionId)
    {
        if (!IsValidWalletNumber(walletNumber))
        {
            return "Enter a valid Bangladeshi mobile number, for example 01712345678.";
        }

        if (!IsValidTransactionId(transactionId))
        {
            return "Enter the Transaction ID (TrxID) from your payment message: 8 to 12 letters or digits.";
        }

        return null;
    }
}
