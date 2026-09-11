using MaleFashion.Application.DTOs;
using MediatR;

namespace MaleFashion.Application.Features.Dashboard.Queries.GetDashboardSummary;

public class GetDashboardSummaryQuery : IRequest<DashboardSummaryDto>
{
}