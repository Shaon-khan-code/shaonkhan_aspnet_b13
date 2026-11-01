using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace MaleFashion.Web.ViewComponents;

public sealed record AdminMenuEntry(
    string Title,
    string? Icon,
    string? Url,
    bool IsActive,
    bool IsExternal,
    IReadOnlyList<AdminMenuEntry> Children);

public sealed record AdminMenuGroup(string? Header, IReadOnlyList<AdminMenuEntry> Entries);

public sealed record AdminMenuViewModel(IReadOnlyList<AdminMenuGroup> Groups);


public class AdminMenuViewComponent : ViewComponent
{
    private sealed record TemplatePage(string Title, string Id);

    private sealed record TemplateEntry(string? Header, string Title, string Icon, TemplatePage[] Pages);

    private sealed record StoreItem(string Title, string Icon, string Controller);

    private sealed record StoreGroup(string? Header, StoreItem[] Items);

    
    private static readonly StoreGroup[] StoreGroups =
    {
        new(null, new[]
        {
            new StoreItem("Dashboard", "bx-home-smile", "Dashboard"),
        }),
        new("Catalog", new[]
        {
            new StoreItem("Products", "bx-package", "Products"),
            new StoreItem("Categories", "bx-category", "Categories"),
            new StoreItem("Brands", "bx-purchase-tag-alt", "Brands"),
            new StoreItem("Tags", "bx-purchase-tag", "Tags"),
            new StoreItem("Inventory", "bx-box", "Inventory"),
            new StoreItem("Blog Posts", "bx-news", "BlogPosts"),
        }),
        new("Variations", new[]
        {
            new StoreItem("Sizes", "bx-ruler", "Sizes"),
            new StoreItem("Colors", "bx-palette", "Colors"),
            new StoreItem("Units", "bx-cube", "Units"),
        }),
        new("Sales", new[]
        {
            new StoreItem("Orders", "bx-cart", "Orders"),
            new StoreItem("Coupons", "bx-gift", "Coupons"),
            new StoreItem("Delivery Zones", "bx-map", "DeliveryZones"),
        }),
        new("People", new[]
        {
            new StoreItem("Customers", "bx-user", "Customers"),
            new StoreItem("Users", "bx-group", "Users"),
            new StoreItem("Roles", "bx-shield", "Roles"),
        }),
    };

    private static readonly TemplateEntry[] TemplateEntries =
    {
        new("Template pages", "Layouts", "bx-layout", new[]
        {
            new TemplatePage("Without menu", "layouts-without-menu"),
            new TemplatePage("Without navbar", "layouts-without-navbar"),
            new TemplatePage("Fluid", "layouts-fluid"),
            new TemplatePage("Container", "layouts-container"),
            new TemplatePage("Blank", "layouts-blank"),
        }),
        new(null, "Account Settings", "bx-dock-top", new[]
        {
            new TemplatePage("Account", "pages-account-settings-account"),
            new TemplatePage("Notifications", "pages-account-settings-notifications"),
            new TemplatePage("Connections", "pages-account-settings-connections"),
        }),
        new(null, "Authentications", "bx-lock-open-alt", new[]
        {
            new TemplatePage("Login", "auth-login-basic"),
            new TemplatePage("Register", "auth-register-basic"),
            new TemplatePage("Forgot Password", "auth-forgot-password-basic"),
        }),
        new(null, "Misc", "bx-cube-alt", new[]
        {
            new TemplatePage("Error", "pages-misc-error"),
            new TemplatePage("Under Maintenance", "pages-misc-under-maintenance"),
        }),
        new("Components", "Cards", "bx-collection", new[] { new TemplatePage("Cards", "cards-basic") }),
        new(null, "User interface", "bx-box", new[]
        {
            new TemplatePage("Accordion", "ui-accordion"),
            new TemplatePage("Alerts", "ui-alerts"),
            new TemplatePage("Badges", "ui-badges"),
            new TemplatePage("Buttons", "ui-buttons"),
            new TemplatePage("Carousel", "ui-carousel"),
            new TemplatePage("Collapse", "ui-collapse"),
            new TemplatePage("Dropdowns", "ui-dropdowns"),
            new TemplatePage("Footer", "ui-footer"),
            new TemplatePage("List groups", "ui-list-groups"),
            new TemplatePage("Modals", "ui-modals"),
            new TemplatePage("Navbar", "ui-navbar"),
            new TemplatePage("Offcanvas", "ui-offcanvas"),
            new TemplatePage("Pagination & Breadcrumbs", "ui-pagination-breadcrumbs"),
            new TemplatePage("Progress", "ui-progress"),
            new TemplatePage("Spinners", "ui-spinners"),
            new TemplatePage("Tabs & Pills", "ui-tabs-pills"),
            new TemplatePage("Toasts", "ui-toasts"),
            new TemplatePage("Tooltips & Popovers", "ui-tooltips-popovers"),
            new TemplatePage("Typography", "ui-typography"),
        }),
        new(null, "Extended UI", "bx-copy", new[]
        {
            new TemplatePage("Perfect Scrollbar", "extended-ui-perfect-scrollbar"),
            new TemplatePage("Text Divider", "extended-ui-text-divider"),
        }),
        new(null, "Boxicons", "bx-crown", new[] { new TemplatePage("Boxicons", "icons-boxicons") }),
        new("Forms & Tables", "Form Elements", "bx-detail", new[]
        {
            new TemplatePage("Basic Inputs", "forms-basic-inputs"),
            new TemplatePage("Input groups", "forms-input-groups"),
        }),
        new(null, "Form Layouts", "bx-detail", new[]
        {
            new TemplatePage("Vertical Form", "form-layouts-vertical"),
            new TemplatePage("Horizontal Form", "form-layouts-horizontal"),
        }),
        new(null, "Tables", "bx-table", new[] { new TemplatePage("Tables", "tables-basic") }),
    };

    private readonly IActionDescriptorCollectionProvider _actionProvider;
    private readonly ICompositeViewEngine _viewEngine;

    public AdminMenuViewComponent(
        IActionDescriptorCollectionProvider actionProvider,
        ICompositeViewEngine viewEngine)
    {
        _actionProvider = actionProvider;
        _viewEngine = viewEngine;
    }

    public IViewComponentResult Invoke()
    {
        var routeValues = ViewContext.RouteData.Values;
        var inAdminArea = string.Equals(routeValues["area"]?.ToString(), "Admin", StringComparison.OrdinalIgnoreCase);
        var currentController = routeValues["controller"]?.ToString();
        var currentId = routeValues["id"]?.ToString();
        var onTemplatePage = inAdminArea && Same(currentController, "Template");

        var groups = BuildStoreGroups(inAdminArea, currentController);

        string? header = null;
        var entries = new List<AdminMenuEntry>();

        foreach (var entry in TemplateEntries)
        {
            if (entry.Header is not null)
            {
                Flush(groups, header, entries);
                header = entry.Header;
                entries = new List<AdminMenuEntry>();
            }

            var pages = entry.Pages.Where(p => TemplatePageExists(p.Id)).ToList();
            if (pages.Count == 0)
            {
                continue;
            }

            if (pages.Count == 1 && entry.Pages.Length == 1)
            {
                var page = pages[0];
                entries.Add(new AdminMenuEntry(entry.Title, entry.Icon, PageUrl(page.Id),
                    onTemplatePage && Same(currentId, page.Id), false, Array.Empty<AdminMenuEntry>()));
                continue;
            }

            var children = pages
                .Select(p => new AdminMenuEntry(p.Title, null, PageUrl(p.Id),
                    onTemplatePage && Same(currentId, p.Id), false, Array.Empty<AdminMenuEntry>()))
                .ToList();

            entries.Add(new AdminMenuEntry(entry.Title, entry.Icon, null,
                children.Any(c => c.IsActive), false, children));
        }

        Flush(groups, header, entries);

        groups.Add(new AdminMenuGroup("Misc", new[]
        {
            new AdminMenuEntry("Support", "bx-support",
                "https://github.com/themeselection/sneat-bootstrap-html-admin-template-free/issues",
                false, true, Array.Empty<AdminMenuEntry>()),
            new AdminMenuEntry("Documentation", "bx-file",
                "https://demos.themeselection.com/sneat-bootstrap-html-admin-template/documentation/",
                false, true, Array.Empty<AdminMenuEntry>()),
        }));

        return View(new AdminMenuViewModel(groups));
    }

    private List<AdminMenuGroup> BuildStoreGroups(bool inAdminArea, string? currentController)
    {
        var available = _actionProvider.ActionDescriptors.Items
            .OfType<ControllerActionDescriptor>()
            .Where(a => a.RouteValues.TryGetValue("area", out var area) && Same(area, "Admin"))
            .Select(a => (Controller: a.ControllerName, Action: a.ActionName))
            .ToList();

        var groups = new List<AdminMenuGroup>();

        foreach (var group in StoreGroups)
        {
            var entries = new List<AdminMenuEntry>();

            foreach (var item in group.Items)
            {
                if (!available.Any(a => Same(a.Controller, item.Controller) && Same(a.Action, "Index")))
                {
                    continue;
                }

                var url = Url.Action("Index", item.Controller, new { area = "Admin" });
                entries.Add(new AdminMenuEntry(item.Title, item.Icon, url,
                    inAdminArea && Same(currentController, item.Controller), false, Array.Empty<AdminMenuEntry>()));
            }

            if (entries.Count > 0)
            {
                groups.Add(new AdminMenuGroup(group.Header, entries));
            }
        }

        return groups;
    }

    private static void Flush(List<AdminMenuGroup> groups, string? header, List<AdminMenuEntry> entries)
    {
        if (entries.Count > 0)
        {
            groups.Add(new AdminMenuGroup(header, entries));
        }
    }

    private bool TemplatePageExists(string id)
        => _viewEngine.GetView(null, $"~/Areas/Admin/Views/Template/{id}.cshtml", isMainPage: false).Success;

    private string? PageUrl(string id)
        => Url.Action("Page", "Template", new { area = "Admin", id });

    private static bool Same(string? a, string? b)
        => string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
}
