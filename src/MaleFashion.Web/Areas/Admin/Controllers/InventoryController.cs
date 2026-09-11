using FluentValidation;
using MaleFashion.Application.Common.Exceptions;
using MaleFashion.Application.Features.Inventory.Commands.UpdateInventory;
using MaleFashion.Application.Features.Inventory.Queries.GetInventoryByProductId;
using MaleFashion.Application.Features.Inventory.Queries.GetInventoryList;
using MaleFashion.Domain.Constants;
using MaleFashion.Web.Areas.Admin.ViewModels.Inventory;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MaleFashion.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = RoleNames.Admin)]
public class InventoryController : Controller
{
    private readonly IMediator _mediator;

    public InventoryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<IActionResult> Index(bool? lowStockOnly, int pageNumber = 1)
    {
        var result = await _mediator.Send(new GetInventoryListQuery
        {
            LowStockOnly = lowStockOnly,
            PageNumber = pageNumber,
            PageSize = 10
        });

        var model = new InventoryListViewModel
        {
            Items = result,
            LowStockOnly = lowStockOnly
        };

        return View(model);
    }

    public async Task<IActionResult> UpdateStock(int id)
    {
        // GetInventoryByProductIdQuery is keyed by ProductId, not Inventory.Id —
        // reusing IInventoryRepository.GetByIdAsync via a direct fetch here would
        // duplicate logic, so instead this action expects the Inventory Id and
        // looks it up the same way the list already has it (via the DTO's Id).
        var inventory = await GetInventoryByIdOrNotFoundAsync(id);
        if (inventory == null)
        {
            return NotFound();
        }

        var model = new UpdateStockViewModel
        {
            Id = inventory.Id,
            ProductName = inventory.ProductName,
            ProductSku = inventory.ProductSku,
            QuantityInStock = inventory.QuantityInStock,
            ReorderLevel = inventory.ReorderLevel
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStock(UpdateStockViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await _mediator.Send(new UpdateInventoryCommand
            {
                Id = model.Id,
                QuantityInStock = model.QuantityInStock,
                ReorderLevel = model.ReorderLevel
            });

            TempData["Success"] = "Stock updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (ValidationException ex)
        {
            foreach (var error in ex.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }
            return View(model);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    private async Task<Application.DTOs.InventoryDto?> GetInventoryByIdOrNotFoundAsync(int inventoryId)
    {
        // No GetInventoryById(int id) query exists yet — only GetByProductId.
        // Rather than invent a redundant query solely for this lookup, the
        // list is paged through here using a large page size as a pragmatic
        // stand-in. Flagged below as worth a dedicated query if this becomes
        // a bottleneck.
        var page = await _mediator.Send(new GetInventoryListQuery { PageNumber = 1, PageSize = int.MaxValue });
        return page.Items.FirstOrDefault(i => i.Id == inventoryId);
    }
}