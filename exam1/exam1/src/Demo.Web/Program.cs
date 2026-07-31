using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using Cortex.Mediator.DependencyInjection;
using Demo.Application.Features.Products.Command;
using Demo.Infrastructure;
using Demo.Infrastructure.Data;
using Demo.Infrastructure.Extensions;
using Demo.Web;
using Demo.Web.Codes;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Reflection;

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    var migrationAssembly = typeof(ApplicationDbContext).Assembly;

    #region Dependency Injection

    builder.Services.AddInfrastructureDependency(builder.Environment.WebRootPath);

    #endregion

    #region Autofac Configuration
    //builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
    //builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
    //{
    //    containerBuilder.RegisterModule(new WebModule(connectionString));
    //});

    #endregion

    #region Cortex Mediator Configuration

    builder.Services.AddCortexMediator(
        new[] {typeof(Program), typeof(ProductAddCommand) },
        options => options.AddDefaultBehaviors()
    );

    #endregion

    #region Mapster Configuration

    // Custom Configuration
    //var config = TypeAdapterConfig.GlobalSettings;
    //config.Scan(typeof(MapsterConfiguration).Assembly);
    //builder.Services.AddSingleton(config);
    //builder.Services.AddScoped<IMapper, ServiceMapper>();

    // Default Configuration
    builder.Services.AddMapster();

    #endregion

    #region DbContext Configuration
    builder.Services.AddDbContext(connectionString, migrationAssembly);
    #endregion

    builder.Services.AddDatabaseDeveloperPageExceptionFilter();

    #region Identity Configuration
    builder.Services.AddIdentity();
    builder.Services.AddPolicyAuthorization();
    #endregion

    builder.Services.AddControllersWithViews();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseMigrationsEndPoint();
    }
    else
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }


    app.UseHttpsRedirection();
    app.UseRouting();

    app.UseAuthorization();

    app.MapStaticAssets();

    app.MapControllerRoute(
        name: "area",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}")
        .WithStaticAssets();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
        .WithStaticAssets();

    Log.Information("Starting application");

    app.Run();
}
catch(Exception e)
{
    Console.WriteLine(e);
    throw;
}
finally
{
    Log.CloseAndFlush();
}
