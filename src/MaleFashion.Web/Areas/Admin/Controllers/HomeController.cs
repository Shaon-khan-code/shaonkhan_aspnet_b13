using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MaleFashion.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        public IActionResult Index() => RedirectToAction("Index", "Dashboard");
    }
    //[Area("Admin")]
    //public class HomeController : Controller
    //{
    //    public IActionResult Index()
    //    {
    //        return View();
    //    }
    //}
}
