using System.ComponentModel.DataAnnotations;

namespace MaleFashion.Web.ViewModels.Checkout;

public class CheckoutViewModel
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

    
    public object? Cart { get; set; }
}
