using FluentValidation;
using MaleFashion.Application;
using MaleFashion.Application.Common.Models;
using MaleFashion.Infrastructure;
using MaleFashion.Infrastructure.Identity;
using MaleFashion.Infrastructure.Options;
using MaleFashion.Web.Middleware;
using MaleFashion.Web.ViewModels.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Serilog;

// Bootstrap logger: captures any startup failure before the full host/config
// is built. Replaced by the fully-configured Serilog logger once the host starts.
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) =>
    {
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            // Defensive masking: if any of these types are ever logged
            // directly (e.g. "{@Card}"), sensitive fields are redacted
            // rather than relying on every call site to remember not to.
            .Destructure.ByTransforming<SimulatedCardDetails>(c => new
            {
                c.CardholderName,
                CardNumber = "***MASKED***",
                Cvv = "***"
            })
            .Destructure.ByTransforming<RegisterViewModel>(m => new { m.FullName, m.Email })
            .Destructure.ByTransforming<LoginViewModel>(m => new { m.Email })
            .Destructure.ByTransforming<ChangePasswordViewModel>(_ => new { Redacted = true })
            .Destructure.ByTransforming<ResetPasswordViewModel>(m => new { m.Email })
            .Destructure.ByTransforming<AwsOptions>(_ => new { Redacted = "AWS credentials" })
            .Destructure.ByTransforming<RecaptchaOptions>(o => new { o.SiteKey, o.MinimumScore })
            .Destructure.ByTransforming<EmailOptions>(o => new { o.Host, o.FromEmail });
    });

    builder.Services.AddControllersWithViews();
    builder.Services.AddControllersWithViews(options =>
    {
        options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
    });

    builder.Services.AddApplicationServices();
    builder.Services.AddInfrastructureServices(builder.Configuration);

    var app = builder.Build();

    app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    app.UseSerilogRequestLogging();

    app.UseHttpsRedirection();
    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapStaticAssets();
    
    app.MapControllerRoute(
        name: "areas",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}")
        .WithStaticAssets();
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
        .WithStaticAssets();

    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var logger = services.GetRequiredService<ILogger<Program>>();

        await IdentitySeeder.SeedRolesAsync(roleManager);
        await IdentitySeeder.SeedAdminUserAsync(userManager, app.Configuration, logger);
    }

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "MaleFashion.Web terminated unexpectedly during startup");
}
finally
{
    Log.CloseAndFlush();
}