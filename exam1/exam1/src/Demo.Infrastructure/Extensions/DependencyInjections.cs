using Demo.Application.Contracts;
using Demo.Application.Contracts.Repositories;
using Demo.Application.Contracts.Services;
using Demo.Infrastructure.Data;
using Demo.Infrastructure.Data.Repositories;
using Demo.Infrastructure.Identity.Requirements;
using Demo.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Infrastructure.Extensions
{
    public static class DependencyInjections
    {
        public static IServiceCollection AddInfrastructureDependency(this IServiceCollection services, string rootPath)
        {
            services.AddScoped<IProductRepository, ProductRepository>();

            services.AddScoped<IApplicationUnitOfWork, ApplicationUnitOfWork>();

            services.AddSingleton<IFileStorageService, FileStorageService>(x => new FileStorageService(rootPath));
            services.AddSingleton<IServerTime, ServerTime>();
            services.AddSingleton<IAuthorizationHandler, AgeRequirementHandler>();
            
            return services;
        }
    }
}
