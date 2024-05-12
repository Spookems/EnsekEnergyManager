using EnsekEnergyManager.Infrastructure.Persistence.Context;
using EnsekEnergyManager.Infrastructure.Persistence.Init;
using Infrastructure.Persistence.Init;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EnsekEnergyManager.Infrastructure
{
    public static class Startup
    {
        internal static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration config)
        {
            return services
            .AddTransient<CustomSeederRunner>();
        }

        public static void ConfigureServices(IServiceCollection services, IConfiguration config)
        {
            // Configure DbContext with connection string
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            // Other services registration...
        }
    }
}