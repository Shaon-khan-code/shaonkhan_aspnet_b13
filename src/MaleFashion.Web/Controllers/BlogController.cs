using Microsoft.AspNetCore.Mvc;

namespace MaleFashion.Web.Controllers;

public class BlogController : Controller
{
    public IActionResult Index() => View();
}