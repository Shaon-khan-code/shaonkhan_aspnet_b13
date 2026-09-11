namespace MaleFashion.Application.Contracts.Services;

public interface IUserStatisticsService
{
    Task<int> GetCustomerCountAsync();
}