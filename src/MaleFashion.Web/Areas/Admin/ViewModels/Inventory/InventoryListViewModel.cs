using MaleFashion.Application.Common.Models;
using MaleFashion.Application.DTOs;

namespace MaleFashion.Web.Areas.Admin.ViewModels.Inventory;

public class InventoryListViewModel
{
    public PagedResult<InventoryDto> Items { get; set; } = new();
    public bool? LowStockOnly { get; set; }
}