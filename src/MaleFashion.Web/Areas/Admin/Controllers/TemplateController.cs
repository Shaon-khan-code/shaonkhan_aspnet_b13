using System.Text.RegularExpressions;
using MaleFashion.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace MaleFashion.Web.Areas.Admin.Controllers;


[Area("Admin")]
[Authorize(Roles = RoleNames.Admin)]
public class TemplateController : Controller
{
    private const string ViewFolder = "~/Areas/Admin/Views/Template/";

    
    private static readonly Regex SafeName = new("^[a-z0-9-]+$", RegexOptions.Compiled);

    private readonly ICompositeViewEngine _viewEngine;

    public TemplateController(ICompositeViewEngine viewEngine)
    {
        _viewEngine = viewEngine;
    }

    [HttpGet]
    public IActionResult Page(string id)
    {
        if (string.IsNullOrWhiteSpace(id) || !SafeName.IsMatch(id))
        {
            return NotFound();
        }

        var viewPath = $"{ViewFolder}{id}.cshtml";
        if (!_viewEngine.GetView(executingFilePath: null, viewPath: viewPath, isMainPage: false).Success)
        {
            return NotFound();
        }

        return View(viewPath);
    }
}
