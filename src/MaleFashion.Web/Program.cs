using MaleFashion.Application;
using MaleFashion.Infrastructure;
using MaleFashion.Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
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

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var logger = services.GetRequiredService<ILogger<Program>>();

    await IdentitySeeder.SeedRolesAsync(roleManager);
    await IdentitySeeder.SeedAdminUserAsync(userManager, app.Configuration, logger);

}

using (var scope = app.Services.CreateScope())
{
    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
    // Replace 1 with a real Product Id you've created.
    var result = await mediator.Send(new MaleFashion.Application.Features.Inventory.Commands.CreateInventory.CreateInventoryCommand
    {
        ProductId = 1,
        QuantityInStock = 5,
        ReorderLevel = 10
    });
    Console.WriteLine($"Created inventory {result.Id}: Available={result.AvailableQuantity}, LowStock={result.IsLowStock}");
}

app.Run();