﻿using EnsekEnergyManager.Infrastructure.Persistence.Context;
using EnsekEnergyManager.Infrastructure.Persistence.Init;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence.Init
{
    internal class ApplicationDbSeeder
    {
        private readonly CustomSeederRunner _seederRunner;
        private readonly ILogger<ApplicationDbSeeder> _logger;

        public ApplicationDbSeeder(ILogger<ApplicationDbSeeder> logger)
        {
            _logger = logger;
        }

        public async Task SeedDatabaseAsync(ApplicationDbContext dbContext, CancellationToken cancellationToken)
        {
            await SeedMeterReadings(dbContext);
            await SeedAccountReadings(dbContext);
            await _seederRunner.RunSeedersAsync(cancellationToken);
        }

        private async Task SeedMeterReadings(ApplicationDbContext dbContext)
        {
            
        }
        private async Task SeedAccountReadings(ApplicationDbContext dbContext)
        {

        }
    }
}
