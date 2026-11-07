using MaleFashion.Application.Common.Helpers;
using Shouldly;
using Xunit;

namespace MaleFashion.Application.UnitTests.Common;

public class MobileWalletValidatorTests
{
    [Theory]
    [InlineData("01712345678", "01712345678")]
    [InlineData("+8801712345678", "01712345678")]
    [InlineData("8801712345678", "01712345678")]
    [InlineData("017 1234-5678", "01712345678")]
    [InlineData("01912345678", "01912345678")]
    public void Normalize_returns_the_local_number_for_valid_input(string input, string expected)
    {
        MobileWalletValidator.Normalize(input).ShouldBe(expected);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("01212345678")]
    [InlineData("0171234567")]
    [InlineData("017123456789")]
    [InlineData("abcdefghijk")]
    public void Normalize_returns_null_for_invalid_input(string? input)
    {
        MobileWalletValidator.Normalize(input).ShouldBeNull();
    }

    [Theory]
    [InlineData("ABC12345XY")]
    [InlineData("8N7A6D5E")]
    [InlineData("abcdefgh")]
    [InlineData("  ABC12345XY  ")]
    public void IsValidTransactionId_accepts_8_to_12_letters_or_digits(string transactionId)
    {
        MobileWalletValidator.IsValidTransactionId(transactionId).ShouldBeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("ABC1234")]
    [InlineData("ABC12345XYZ99")]
    [InlineData("ABC-12345")]
    public void IsValidTransactionId_rejects_wrong_length_or_characters(string? transactionId)
    {
        MobileWalletValidator.IsValidTransactionId(transactionId).ShouldBeFalse();
    }

    [Fact]
    public void Validate_returns_null_when_number_and_transaction_id_are_valid()
    {
        MobileWalletValidator.Validate("01712345678", "ABC12345XY").ShouldBeNull();
    }

    [Fact]
    public void Validate_reports_the_wallet_number_first_when_it_is_invalid()
    {
        MobileWalletValidator.Validate("123", "ABC12345XY")!.ShouldContain("mobile number");
    }

    [Fact]
    public void Validate_reports_the_transaction_id_when_only_it_is_invalid()
    {
        MobileWalletValidator.Validate("01712345678", "xx")!.ShouldContain("Transaction ID");
    }
}
