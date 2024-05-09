using Infrastructure.Persistence.Init;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnsekEnergyManager.Infrastructure.Persistence.Init
{
    internal class DatabaseInitialiser : IDatabaseInitializer
    {
        private readonly DatabaseSettings _dbSettings;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DatabaseInitialiser> _logger;

        public DatabaseInitialiser(IOptions<DatabaseSettings> dbSettings, IServiceProvider serviceProvider, ILogger<DatabaseInitialiser> logger)
        {
            _dbSettings = dbSettings.Value;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task InitializeDatabasesAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Init Ensek db");
            using var scope = _serviceProvider.CreateScope();

            // Then run the initialization in the new scope
            await scope.ServiceProvider.GetRequiredService<ApplicationDbInitialiser>()
                .InitializeAsync(cancellationToken);
        }
    }
}
