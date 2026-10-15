using MaleFashion.Application.Features.Products.Queries.GetProductList;
using MaleFashion.Web.Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MaleFashion.Web.ViewComponents;

public sealed record DealOfTheWeekViewModel(object Product, DateTime DeadlineUtc);

public class DealOfTheWeekViewComponent : ViewComponent
{
    private readonly IMediator _mediator;
    private readonly ILogger<DealOfTheWeekViewComponent> _logger;

    public DealOfTheWeekViewComponent(IMediator mediator, ILogger<DealOfTheWeekViewComponent> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        try
        {
            var result = await _mediator.Send(new GetProductListQuery
            {
                IsActive = true,
                SortBy = ProductSortOption.NameAsc,
                PageNumber = 1,
                PageSize = 24
            });

            var products = result.GetList("Items", "Data", "Results", "Products", "Records");
            var deal = products.FirstOrDefault(p => p.IsOnSale()) ?? products.FirstOrDefault();

            return deal is null
                ? Content(string.Empty)
                : View(new DealOfTheWeekViewModel(deal, EndOfWeekUtc(DateTime.UtcNow)));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not load the deal of the week.");
            return Content(string.Empty);
        }
    }

    
    private static DateTime EndOfWeekUtc(DateTime now)
    {
        var daysUntilSunday = ((int)DayOfWeek.Sunday - (int)now.DayOfWeek + 7) % 7;
        return now.Date.AddDays(daysUntilSunday + 1).AddSeconds(-1);
    }
}
