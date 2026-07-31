using Cortex.Mediator;
using Demo.Application.Contracts;
using Demo.Application.Features.Products.Command;
using Demo.Domain.Entities;
using Demo.Web.Codes;
using Demo.Web.Models;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Diagnostics;
using System.Runtime.InteropServices.Marshalling;

namespace Demo.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public HomeController(ILogger<HomeController> logger, IMediator mediator, IMapper mapper)
        {
            _logger = logger;
            _mediator = mediator;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            Log.Debug("I am in home page");
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
