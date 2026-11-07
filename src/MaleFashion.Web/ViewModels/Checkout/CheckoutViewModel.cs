using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace MaleFashion.Web.ViewModels.Checkout;

public enum CheckoutPaymentMethod
{
    CashOnDelivery = 0,
    Bkash = 1,
    Nagad = 2,
    Card = 3
}

public class CheckoutViewModel : IValidatableObject
{
    [Required(ErrorMessage = "First name is required.")]
    [StringLength(60)]
    [Display(Name = "First name")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(60)]
    [Display(Name = "Last name")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Country is required.")]
    [StringLength(80)]
    public string Country { get; set; } = string.Empty;

    [Required(ErrorMessage = "Street address is required.")]
    [StringLength(200)]
    [Display(Name = "Street address")]
    public string AddressLine1 { get; set; } = string.Empty;

    [StringLength(200)]
    [Display(Name = "Apartment, suite, unit (optional)")]
    public string? AddressLine2 { get; set; }

    [Required(ErrorMessage = "Town / city is required.")]
    [StringLength(80)]
    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "State / region is required.")]
    [StringLength(80)]
    public string State { get; set; } = string.Empty;

    [Required(ErrorMessage = "Postcode / ZIP is required.")]
    [StringLength(20)]
    public string PostalCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required.")]
    [Phone(ErrorMessage = "Please enter a valid phone number.")]
    [StringLength(30)]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    [StringLength(200)]
    public string Email { get; set; } = string.Empty;

    [StringLength(500)]
    public string? OrderNotes { get; set; }

    [Display(Name = "Payment method")]
    public CheckoutPaymentMethod PaymentMethod { get; set; } = CheckoutPaymentMethod.CashOnDelivery;

    
    [StringLength(20)]
    [Display(Name = "Your wallet number")]
    public string? WalletNumber { get; set; }

  
    [StringLength(20)]
    [Display(Name = "Transaction ID")]
    public string? TransactionId { get; set; }


    [StringLength(80)]
    [Display(Name = "Name on card")]
    public string? CardholderName { get; set; }

    [StringLength(30)]
    [Display(Name = "Card number")]
    public string? CardNumber { get; set; }

    [StringLength(2)]
    [Display(Name = "Expiry month")]
    public string? ExpiryMonth { get; set; }

    [StringLength(4)]
    [Display(Name = "Expiry year")]
    public string? ExpiryYear { get; set; }

    [StringLength(4)]
    public string? Cvv { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (PaymentMethod == CheckoutPaymentMethod.Card)
        {
            if (string.IsNullOrWhiteSpace(CardholderName))
            {
                yield return new ValidationResult("Enter the name on the card.", new[] { nameof(CardholderName) });
            }

            if (string.IsNullOrWhiteSpace(CardNumber))
            {
                yield return new ValidationResult("Enter the card number.", new[] { nameof(CardNumber) });
            }

            if (string.IsNullOrWhiteSpace(ExpiryMonth) || string.IsNullOrWhiteSpace(ExpiryYear))
            {
                yield return new ValidationResult("Choose the card expiry month and year.", new[] { nameof(ExpiryMonth) });
            }

            if (string.IsNullOrWhiteSpace(Cvv))
            {
                yield return new ValidationResult("Enter the CVV.", new[] { nameof(Cvv) });
            }
        }

        if (PaymentMethod is CheckoutPaymentMethod.Bkash or CheckoutPaymentMethod.Nagad)
        {
            var wallet = (WalletNumber ?? string.Empty).Replace(" ", string.Empty).Replace("-", string.Empty);
            if (!Regex.IsMatch(wallet, @"^(\+?88)?01[3-9]\d{8}$"))
            {
                yield return new ValidationResult(
                    "Enter a valid Bangladeshi mobile number, for example 01712345678.",
                    new[] { nameof(WalletNumber) });
            }

            if (!Regex.IsMatch(TransactionId ?? string.Empty, "^[A-Za-z0-9]{8,12}$"))
            {
                yield return new ValidationResult(
                    "Enter the Transaction ID (TrxID) from your payment message: 8 to 12 letters or digits.",
                    new[] { nameof(TransactionId) });
            }
        }
    }

   
    public object? Cart { get; set; }
}
