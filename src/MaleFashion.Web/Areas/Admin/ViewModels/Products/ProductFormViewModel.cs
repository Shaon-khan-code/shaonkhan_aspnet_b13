using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MaleFashion.Web.Areas.Admin.ViewModels.Products;

public class ProductFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Slug is required.")]
    [StringLength(220)]
    public string Slug { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    [Required(ErrorMessage = "SKU is required.")]
    [StringLength(50)]
    public string Sku { get; set; } = string.Empty;

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
    public decimal Price { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Discount price must be greater than zero.")]
    public decimal? DiscountPrice { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;

    [Required(ErrorMessage = "Please select a category.")]
    [Display(Name = "Category")]
    public int CategoryId { get; set; }

    public List<SelectListItem> CategoryOptions { get; set; } = new();

    [Display(Name = "Product Image")]
    public IFormFile? ImageFile { get; set; }

    public string? CurrentImageUrl { get; set; }
}