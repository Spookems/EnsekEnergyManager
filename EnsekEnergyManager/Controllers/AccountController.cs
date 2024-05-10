using EnsekEnergyManager.Infrastructure.Persistence.Init;
using Microsoft.AspNetCore.Mvc;

namespace EnsekEnergyManager.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        CustomSeederRunner _seeder;

        public AccountController(CustomSeederRunner seeder)
        {
            _seeder = seeder;
        }

        [HttpGet(Name = "Seed DB")]
        public void SeedFromInternalCSVs(CancellationToken cancellationToken)
        {
            _ = _seeder.RunSeedersAsync(cancellationToken); // replace with request
        }
    }
}
