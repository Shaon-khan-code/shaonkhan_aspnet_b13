namespace MaleFashion.Web.Areas.Admin.ViewModels.Customers;

public class CustomerListItemViewModel
{
    public string Id { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? FullName { get; set; }
    public string? PhoneNumber { get; set; }
    public bool IsLockedOut { get; set; }
}