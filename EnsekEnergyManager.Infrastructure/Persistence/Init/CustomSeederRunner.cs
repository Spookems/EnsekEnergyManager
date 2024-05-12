using Microsoft.Extensions.DependencyInjection;

namespace EnsekEnergyManager.Infrastructure.Persistence.Init
{
    public class CustomSeederRunner
    {
        private readonly ICustomSeeder[] _seeders;

        public CustomSeederRunner()
        {
        }

        public CustomSeederRunner(IServiceProvider serviceProvider) =>
        _seeders = serviceProvider.GetServices<ICustomSeeder>().ToArray();

        public async Task RunSeedersAsync(CancellationToken cancellationToken)
        {
            foreach (var seeder in _seeders)
            {
                await seeder.InitializeAsync(cancellationToken);
            }
        }
    }
}
