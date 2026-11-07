using FluentValidation;
using MaleFashion.Application.Common.Exceptions;
using MaleFashion.Application.DTOs;
using MaleFashion.Application.Features.Inventory.Commands.UpdateInventory;
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

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var inventory = await FindInventoryAsync(id);
        if (inventory == null)
        {
            return NotFound();
        }

        return View(new UpdateStockViewModel
        {
            Id = inventory.Id,
            ProductName = inventory.ProductName,
            ProductSku = inventory.ProductSku,
            QuantityInStock = inventory.QuantityInStock,
            ReservedQuantity = inventory.ReservedQuantity,
            ReorderLevel = inventory.ReorderLevel
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdateStockViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            await FillDisplayFieldsAsync(model);
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

            await FillDisplayFieldsAsync(model);
            return View(model);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

  
    private async Task<InventoryDto?> FindInventoryAsync(int inventoryId)
    {
        for (var page = 1; ; page++)
        {
            var result = await _mediator.Send(new GetInventoryListQuery
            {
                PageNumber = page,
                PageSize = 50
            });

            var match = result.Items.FirstOrDefault(i => i.Id == inventoryId);
            if (match != null)
            {
                return match;
            }

            if (page >= result.TotalPages)
            {
                return null;
            }
        }
    }

    
    private async Task FillDisplayFieldsAsync(UpdateStockViewModel model)
    {
        var inventory = await FindInventoryAsync(model.Id);
        if (inventory == null)
        {
            return;
        }

        model.ProductName = inventory.ProductName;
        model.ProductSku = inventory.ProductSku;
        model.ReservedQuantity = inventory.ReservedQuantity;
    }
}
