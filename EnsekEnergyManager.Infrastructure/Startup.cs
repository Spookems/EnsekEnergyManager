using EnsekEnergyManager.Infrastructure.Persistence.Init;
using Infrastructure.Persistence.Init;
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
    }
}