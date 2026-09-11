using MaleFashion.Application.Contracts.Repositories;
using MaleFashion.Application.Contracts.Services;
using MaleFashion.Application.DTOs;
using MaleFashion.Domain.Enums;
using MapsterMapper;
using MediatR;

namespace MaleFashion.Application.Features.Dashboard.Queries.GetDashboardSummary;

public class GetDashboardSummaryQueryHandler : IRequestHandler<GetDashboardSummaryQuery, DashboardSummaryDto>
{
    private const int RecentOrderCount = 5;

    private readonly IApplicationUnitOfWork _unitOfWork;
    private readonly IUserStatisticsService _userStatisticsService;
    private readonly IMapper _mapper;

    public GetDashboardSummaryQueryHandler(
        IApplicationUnitOfWork unitOfWork, IUserStatisticsService userStatisticsService, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _userStatisticsService = userStatisticsService;
        _mapper = mapper;
    }

    public async Task<DashboardSummaryDto> Handle(GetDashboardSummaryQuery request, CancellationToken cancellationToken)
    {
        var totalProducts = await _unitOfWork.Products.CountAsync();
        var totalCustomers = await _userStatisticsService.GetCustomerCountAsync();
        var totalOrders = await _unitOfWork.Orders.CountAsync();
        var totalRevenue = await _unitOfWork.Orders.GetTotalRevenueAsync(PaymentStatus.Paid);
        var pendingOrders = await _unitOfWork.Orders.CountAsync(o => o.Status == OrderStatus.Pending);
        var lowStockProducts = await _unitOfWork.Inventory.GetLowStockCountAsync();
        var recentOrders = await _unitOfWork.Orders.GetRecentOrdersAsync(RecentOrderCount);

        return new DashboardSummaryDto
        {
            TotalProducts = totalProducts,
            TotalCustomers = totalCustomers,
            TotalOrders = totalOrders,
            TotalRevenue = totalRevenue,
            PendingOrders = pendingOrders,
            LowStockProducts = lowStockProducts,
            RecentOrders = _mapper.Map<List<OrderListItemDto>>(recentOrders)
        };
    }
}