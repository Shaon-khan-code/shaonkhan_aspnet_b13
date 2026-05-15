using DevSkill.Shop.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace DevSkill.Shop.Web.Controllers
{
    public class TeamController : Controller
    {
        public IActionResult Index()
        {
            var teamMembers = new List<TeamListModel>
            {
                new TeamListModel
                {
                    Name = "Sohel ",
                    Designation = "Software Engineer",
                    Email = "sohel@example.com",
                    ImageUrl = "https://via.placeholder.com/300"
                },
                new TeamListModel
                {
                    Name = "Shaon Khan",
                    Designation = "Project Manager",
                    Email = "shaon@example.com",
                    ImageUrl = "https://via.placeholder.com/300"
                },
                new TeamListModel
                {
                    Name = "Sakib ",
                    Designation = "UI/UX Designer",
                    Email = "sakib@example.com",
                    ImageUrl = "https://via.placeholder.com/300"
                }
            };

            return View(teamMembers);
        }
    }

}

