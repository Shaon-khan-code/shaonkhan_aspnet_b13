using System.ComponentModel.DataAnnotations;

namespace MaleFashion.Web.Areas.Admin.ViewModels.Inventory;

public class UpdateStockViewModel
{
    public int Id { get; set; }

    [Display(Name = "Product Name")]
    public string ProductName { get; set; } = string.Empty;

    [Display(Name = "SKU")]
    public string ProductSku { get; set; } = string.Empty;

    [Required(ErrorMessage = "Quantity in stock is required.")]
    [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative.")]
    [Display(Name = "Quantity in Stock")]
    public int QuantityInStock { get; set; }

    [Required(ErrorMessage = "Reorder level is required.")]
    [Range(0, int.MaxValue, ErrorMessage = "Reorder level cannot be negative.")]
    [Display(Name = "Reorder Level")]
    public int ReorderLevel { get; set; }
    public int ReservedQuantity { get; set; }
}