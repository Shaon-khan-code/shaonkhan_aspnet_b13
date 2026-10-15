using Microsoft.AspNetCore.Mvc;

namespace MaleFashion.Web.Controllers;

public class AboutController : Controller
{
    public IActionResult Index() => View();
}