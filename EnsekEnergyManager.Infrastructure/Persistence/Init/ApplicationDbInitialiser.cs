using EnsekEnergyManager.Infrastructure.Persistence.Context;
using Infrastructure.Persistence.Init;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnsekEnergyManager.Infrastructure.Persistence.Init
{
    internal class ApplicationDbInitialiser
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ApplicationDbSeeder _dbSeeder;
        private readonly ILogger<ApplicationDbInitialiser> _logger;

        public ApplicationDbInitialiser(ApplicationDbContext dbContext, ApplicationDbSeeder dbSeeder, ILogger<ApplicationDbInitialiser> logger)
        {
            _dbContext = dbContext;
            _dbSeeder = dbSeeder;
            _logger = logger;
        }

        public async Task InitializeAsync(CancellationToken cancellationToken)
        {
            if (_dbContext.Database.GetMigrations().Any())
            {
                if ((await _dbContext.Database.GetPendingMigrationsAsync(cancellationToken)).Any())
                {
                    _logger.LogInformation("Applying Migrations for application");
                    await _dbContext.Database.MigrateAsync(cancellationToken);
                }

                if (await _dbContext.Database.CanConnectAsync(cancellationToken))
                {
                    _logger.LogInformation("Connection to Database Succeeded.");

                    await _dbSeeder.SeedDatabaseAsync(_dbContext, cancellationToken);
                }
            }
        }
    }
}
