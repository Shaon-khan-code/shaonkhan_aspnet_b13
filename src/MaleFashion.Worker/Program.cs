using MaleFashion.Application;
using MaleFashion.Infrastructure;
using MaleFashion.Worker;
using MaleFashion.Worker.Options;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<WorkerOptions>(builder.Configuration.GetSection("Worker"));

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.Configure<WorkerOptions>(builder.Configuration.GetSection("Worker"));

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();