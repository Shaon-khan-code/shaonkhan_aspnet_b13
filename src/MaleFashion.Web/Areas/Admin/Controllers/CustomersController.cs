using MaleFashion.Application.Features.Orders.Queries.GetOrdersByUser;
using MaleFashion.Domain.Constants;
using MaleFashion.Infrastructure.Identity;
using MaleFashion.Web.Areas.Admin.ViewModels.Customers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MaleFashion.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = RoleNames.Admin)]
public class CustomersController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMediator _mediator;

    public CustomersController(UserManager<ApplicationUser> userManager, IMediator mediator)
    {
        _userManager = userManager;
        _mediator = mediator;
    }

    public async Task<IActionResult> Index(string? searchTerm, int pageNumber = 1)
    {
        const int pageSize = 10;

        // NOTE: GetUsersInRoleAsync materializes all Customer-role users;
        // search and paging below are applied in-memory, not at the DB level.
        // See design note for the reasoning and the stricter alternative.
        var customers = await _userManager.GetUsersInRoleAsync(RoleNames.Customer);

        var filtered = customers.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            filtered = filtered.Where(u =>
                (u.Email != null && u.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                (u.FullName != null && u.FullName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)));
        }

        var filteredList = filtered.OrderBy(u => u.Email).ToList();
        var totalCount = filteredList.Count;

        var pageItems = filteredList
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var items = new List<CustomerListItemViewModel>();
        foreach (var user in pageItems)
        {
            items.Add(new CustomerListItemViewModel
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                IsLockedOut = await _userManager.IsLockedOutAsync(user)
            });
        }

        ViewData["SearchTerm"] = searchTerm;
        ViewData["PageNumber"] = pageNumber;
        ViewData["TotalPages"] = (int)Math.Ceiling(totalCount / (double)pageSize);

        return View(items);
    }

    public async Task<IActionResult> Details(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var isCustomer = await _userManager.IsInRoleAsync(user, RoleNames.Customer);
        if (!isCustomer)
        {
            // This page is scoped to customers only — never show Admin accounts here.
            return NotFound();
        }

        var orders = await _mediator.Send(new GetOrdersByUserQuery { UserId = user.Id });

        var model = new CustomerDetailsViewModel
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber,
            IsLockedOut = await _userManager.IsLockedOutAsync(user),
            Orders = orders
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleStatus(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var isCustomer = await _userManager.IsInRoleAsync(user, RoleNames.Customer);
        if (!isCustomer)
        {
            return NotFound();
        }

        var isCurrentlyLockedOut = await _userManager.IsLockedOutAsync(user);

        if (isCurrentlyLockedOut)
        {
            // Reactivate: clear lockout. Does NOT touch PasswordHash/SecurityStamp.
            await _userManager.SetLockoutEndDateAsync(user, null);
            TempData["Success"] = "Customer account reactivated.";
        }
        else
        {
            // Deactivate: lock out indefinitely via LockoutEnd, not deletion or
            // password changes. LockoutEnabled is ensured true first, since
            // Identity ignores LockoutEnd entirely when LockoutEnabled is false.
            await _userManager.SetLockoutEnabledAsync(user, true);
            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
            TempData["Success"] = "Customer account deactivated.";
        }

        return RedirectToAction(nameof(Details), new { id });
    }
}