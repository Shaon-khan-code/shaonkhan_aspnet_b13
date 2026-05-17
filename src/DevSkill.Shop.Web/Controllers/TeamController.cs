using DevSkill.Shop.Application.Interfaces;
using DevSkill.Shop.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DevSkill.Shop.Web.Controllers;

public class TeamController : Controller
{
    private readonly IApplicationUnitOfWork _unitOfWork;

    public TeamController(IApplicationUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Insert()
    {
        var team = new Team
        {
            Name = "Shaon Khan",
            Designation = "Software Engineer",
            ImageUrl = "/images/team1.jpg"
        };

        _unitOfWork.Teams.Add(team);

        _unitOfWork.Save();

        return Content("Team Inserted Successfully");
    }
}
