using MaleFashion.Application.Contracts.Services;
using MaleFashion.Domain.Constants;
using MaleFashion.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MaleFashion.Infrastructure.Services.Users;

public class UserStatisticsService : IUserStatisticsService
{
    private readonly ApplicationDbContext _context;

    public UserStatisticsService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> GetCustomerCountAsync()
    {
        var customerRoleId = await _context.Roles
            .Where(r => r.Name == RoleNames.Customer)
            .Select(r => r.Id)
            .FirstOrDefaultAsync();

        if (customerRoleId == null)
        {
            return 0;
        }

        return await _context.UserRoles
            .Where(ur => ur.RoleId == customerRoleId)
            .CountAsync();
    }
}