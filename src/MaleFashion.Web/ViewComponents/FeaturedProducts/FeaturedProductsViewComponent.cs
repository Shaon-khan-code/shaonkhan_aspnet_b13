using MaleFashion.Application.Features.Products.Queries.GetProductList;
using MaleFashion.Web.Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MaleFashion.Web.ViewComponents;

public sealed record FeaturedProductsViewModel(
    IReadOnlyList<object> BestSellers,
    IReadOnlyList<object> NewArrivals,
    IReadOnlyList<object> HotSales);


public class FeaturedProductsViewComponent : ViewComponent
{
    private const int CandidatePoolSize = 24;

    private readonly IMediator _mediator;
    private readonly ILogger<FeaturedProductsViewComponent> _logger;

    public FeaturedProductsViewComponent(IMediator mediator, ILogger<FeaturedProductsViewComponent> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<IViewComponentResult> InvokeAsync(int take = 8)
    {
        try
        {

            var pool = await LoadAsync(ProductSortOption.NameAsc);
            var newest = FindNewestSort() is { } sort ? await LoadAsync(sort) : pool;

            return View(new FeaturedProductsViewModel(
                BestSellers: pool.Take(take).ToList(),
                NewArrivals: newest.Take(take).ToList(),
                HotSales: pool.Where(p => p.IsOnSale()).Take(take).ToList()));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not load featured products for the home page.");
            return View(new FeaturedProductsViewModel(
                Array.Empty<object>(), Array.Empty<object>(), Array.Empty<object>()));
        }
    }

    private async Task<IReadOnlyList<object>> LoadAsync(ProductSortOption sortBy)
    {
        var result = await _mediator.Send(new GetProductListQuery
        {
            IsActive = true,
            SortBy = sortBy,
            PageNumber = 1,
            PageSize = CandidatePoolSize
        });

        return result.GetList("Items", "Data", "Results", "Products", "Records");
    }


    private static ProductSortOption? FindNewestSort()
    {
        var names = Enum.GetNames<ProductSortOption>();

        var match = names.FirstOrDefault(n => n.Contains("Newest", StringComparison.OrdinalIgnoreCase))
                    ?? names.FirstOrDefault(n =>
                        (n.Contains("Created", StringComparison.OrdinalIgnoreCase)
                         || n.Contains("Date", StringComparison.OrdinalIgnoreCase))
                        && n.EndsWith("Desc", StringComparison.OrdinalIgnoreCase));

        return match is null ? null : Enum.Parse<ProductSortOption>(match);
    }
}
