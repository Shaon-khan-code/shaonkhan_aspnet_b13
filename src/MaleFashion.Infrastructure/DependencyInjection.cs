using Amazon;
using Amazon.S3;
using MaleFashion.Application.Contracts.Repositories;
using MaleFashion.Application.Contracts.Services;
using MaleFashion.Infrastructure.Data;
using MaleFashion.Infrastructure.Data.Repositories;
using MaleFashion.Infrastructure.Identity;
using MaleFashion.Infrastructure.Options;
using MaleFashion.Infrastructure.Services.Email;
using MaleFashion.Infrastructure.Services.Payment;
using MaleFashion.Infrastructure.Services.Recaptcha;
using MaleFashion.Infrastructure.Services.S3;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MaleFashion.Infrastructure.Services.Users;


namespace MaleFashion.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.User.RequireUniqueEmail = true;
        })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.Configure<EmailOptions>(configuration.GetSection("Email"));
        services.AddScoped<IEmailService, SmtpEmailService>();

        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IApplicationUnitOfWork, ApplicationUnitOfWork>();
        services.Configure<AwsOptions>(configuration.GetSection("Aws"));
        services.AddScoped<IInventoryRepository, InventoryRepository>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IAddressRepository, AddressRepository>();
        services.AddScoped<ISimulatedPaymentService, SimulatedCardPaymentService>();
        services.AddScoped<IUserStatisticsService, UserStatisticsService>();


        services.AddSingleton<IAmazonS3>(sp =>
        {
            var awsOptions = configuration.GetSection("Aws").Get<AwsOptions>() ?? new AwsOptions();

            if (string.IsNullOrWhiteSpace(awsOptions.AccessKey) || string.IsNullOrWhiteSpace(awsOptions.Region))
            {
                return new AmazonS3Client(RegionEndpoint.USEast1);
            }

            var credentials = new Amazon.Runtime.BasicAWSCredentials(awsOptions.AccessKey, awsOptions.SecretKey);
            return new AmazonS3Client(credentials, RegionEndpoint.GetBySystemName(awsOptions.Region));
        });

        services.AddScoped<IFileStorageService, S3FileStorageService>();

        return services;
    }
}