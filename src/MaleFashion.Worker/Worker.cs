using MaleFashion.Application.Features.Inventory.Queries.GetLowStockProducts;
using MaleFashion.Worker.Options;
using MediatR;
using Microsoft.Extensions.Options;

namespace MaleFashion.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly WorkerOptions _options;

    public Worker(ILogger<Worker> logger, IServiceScopeFactory scopeFactory, IOptions<WorkerOptions> options)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "MaleFashion Worker starting. Poll interval: {IntervalSeconds}s", _options.IntervalSeconds);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CheckInventoryAsync(stoppingToken);

                await Task.Delay(TimeSpan.FromSeconds(_options.IntervalSeconds), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Expected during graceful shutdown — not an error.
            }
        }

        _logger.LogInformation("MaleFashion Worker stopping.");
    }

    private async Task CheckInventoryAsync(CancellationToken cancellationToken)
    {
        // BackgroundService itself is registered as a singleton, but MediatR
        // handlers and the DbContext behind them are scoped — a new scope is
        // created per run so those scoped services resolve correctly here.
        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var lowStockItems = await mediator.Send(new GetLowStockProductsQuery(), cancellationToken);

        if (lowStockItems.Count > 0)
        {
            _logger.LogWarning("Inventory check found {Count} low-stock product(s).", lowStockItems.Count);
        }
        else
        {
            _logger.LogInformation("Inventory check complete. No low-stock products found.");
        }
    }
}