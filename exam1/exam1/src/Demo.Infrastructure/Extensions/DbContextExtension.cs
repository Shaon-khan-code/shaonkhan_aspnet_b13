using Demo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Demo.Infrastructure.Extensions
{
    public static class DbContextExtension
    {
        public static void AddDbContext(this IServiceCollection services, string connectionString, Assembly migrationAssembly)
        {
            services.AddDbContext<ApplicationDbContext>(options => 
                options.UseSqlServer(connectionString, 
                x => x.MigrationsAssembly(migrationAssembly)));
        }
    }
}
