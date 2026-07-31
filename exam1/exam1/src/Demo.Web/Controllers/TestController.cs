using Demo.Web.Codes;
using Demo.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Web.Controllers
{
    public class TestController : Controller
    {
        private readonly IMembership _membership;
        public TestController([FromKeyedServices("Setup 1")] IMembership membership)
        {
            _membership = membership;
        }

        public IActionResult Index()
        {
            var model = new TestHome 
            { 
                Name = "Jalaluddin", 
                Email = "info@devskill.com", 
                PartialModel = new PartialModel { Address = "Dhaka" } 
            };

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Index(TestHome model)
        {
            if (ModelState.IsValid)
            {
                var email = model.Email;
                var name = model.Name;
            }

            return View(model);
        }
    }
}
