using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace MaleFashion.Web.Areas.Admin.ViewModels.Categories;

public class CategoryFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Slug is required.")]
    [StringLength(180)]
    public string Slug { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Display(Name = "Parent Category")]
    public int? ParentCategoryId { get; set; }

    public List<SelectListItem> ParentCategoryOptions { get; set; } = new();
}