using CsvHelper;
using EnsekEnergyManager.Classes;
using EnsekEnergyManager.Infrastructure.Mapping;
using EnsekEnergyManager.Infrastructure.Persistence.Context;
using EnsekEnergyManager.Infrastructure.Persistence.Init;
using EnsekEnergyManager.Infrastructure.Seeders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Formats.Asn1;
using System.Globalization;
using System.Threading;

namespace EnsekEnergyManager.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        CustomSeederRunner _seeder;
        private readonly ApplicationDbContext _db;
        private readonly ILogger<AccountController> _logger;

        public AccountController(CustomSeederRunner seeder, ApplicationDbContext db, ILogger<AccountController> logger)
        {
            _seeder = seeder;
            _db = db;
            _logger = logger;
        }

        [HttpPost(Name = "Seed DB")]
        public void SeedFromInternalCSVs(CancellationToken cancellationToken)
        {
            _ = _seeder.RunSeedersAsync(cancellationToken); // replace with request
        }

        [HttpPost("meter-reading-uploads")]
        public async Task<IActionResult> UploadMeterReadings(IFormFile file, CancellationToken cancellationToken)
        {
            string fileExtension = Path.GetExtension(file.FileName);

            if (!string.IsNullOrEmpty(fileExtension) || fileExtension != ".csv") 
            {
                List<MeterReading> ValidRecords = new List<MeterReading>();
                List<MeterReading> InValidRecords = new List<MeterReading>();

                using (StreamReader stream = new StreamReader(file.OpenReadStream()))
                {
                    CsvReader csvReader = new CsvReader(stream, CultureInfo.InvariantCulture);
                    csvReader.Context.RegisterClassMap<MeterReadingMap>();

                    try
                    {
                        List<MeterReading> records = csvReader.GetRecords<MeterReading>().ToList();
                        List<Account> accounts = await _db.Accounts.ToListAsync();

                        foreach (var record in records)
                        {
                            if (accounts.Any(x => x.AccountId == record.AccountId))
                            {
                                record.Id = Guid.NewGuid();
                                ValidRecords.Add(new MeterReading()
                                {
                                    AccountId = record.AccountId,
                                    MeterReadingDateTime = record.MeterReadingDateTime,
                                    MeterReadValue = record.MeterReadValue
                                });
                            }
                            else
                            {
                                InValidRecords.Add(record);
                            }
                        }

                        await _db.MeterReadings.AddRangeAsync(ValidRecords, cancellationToken);
                        await _db.SaveChangesAsync(cancellationToken);

                        return Ok($"Successfully processed {records.Count} meter readings. \n" +
                            $"Valid Records Found: {ValidRecords?.Count} \n" +
                            $"Invalid Records Found: {InValidRecords?.Count}");
                    }
                    catch (Exception ex)
                    {
                        return BadRequest($"Error processing file: {ex.Message}");
                    }
                }
            }

            return BadRequest($"Error processing file: Expected a csv, recieved: {fileExtension ?? "empty extension"}");
        }
    }
}
