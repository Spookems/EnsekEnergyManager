using CsvHelper;
using EnsekEnergyManager.Classes;
using EnsekEnergyManager.Infrastructure.Mapping;
using EnsekEnergyManager.Infrastructure.Persistence.Context;
using EnsekEnergyManager.Infrastructure.Persistence.Init;
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
            List<MeterReading> ValidRecords = new List<MeterReading>();
            List<MeterReading> InValidRecords = new List<MeterReading>();

            ApplicationDbContext _db = new ApplicationDbContext();


            using (var stream = new StreamReader(file.OpenReadStream()))
            {
                var csvReader = new CsvReader(stream, CultureInfo.InvariantCulture);
                csvReader.Context.RegisterClassMap<MeterReadingMap>();

                try
                {
                    var records = csvReader.GetRecords<MeterReading>().ToList();
                    List<Account> accounts = await _db.Accounts.ToListAsync();

                    foreach (var record in records)
                    {
                        if (accounts.Any(x => x.AccountId == record.AccountId))
                        {
                            record.Id = Guid.NewGuid();
                            ValidRecords.Add(record);
                        }
                        else
                        {
                            InValidRecords.Add(record);
                        }

                        _db.MeterReadings.Add(record);
                    }


                    _logger.LogInformation($"Valid Records Found: {ValidRecords?.Count}");
                    _logger.LogInformation($"Invalid Records Found: {InValidRecords?.Count}");
                    _logger.LogInformation($"Begining Upsert");

                    for (int i = 0; i < ValidRecords?.Count; i++)
                    {
                        MeterReading? record = ValidRecords[i];
                        _db.MeterReadings.Add(record);
                    }

                    await _db.MeterReadings.AddRangeAsync(ValidRecords, cancellationToken);
                    await _db.SaveChangesAsync(cancellationToken);

                    return Ok($"Successfully processed {records.Count} meter readings.");
                }
                catch (Exception ex)
                {
                    return BadRequest($"Error processing file: {ex.Message}");
                }
            }














            
            if (file == null || file.Length <= 0)
                return BadRequest("File is empty.");

            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                using (var csv = new CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture))
                {
                    // Assuming your CSV has headers
                    csv.Context.Configuration.HasHeaderRecord = true;
                    csv.Context.RegisterClassMap<MeterReadingMap>(); // Register the mapping

                    //csv.ReadHeader();
                    //var csvHeaders = csv.HeaderRecord;

                    //var hasHeaderRecord = csvHeaders.Intersect(_ColumnNames).Count() == csvHeaders.Count();


                    var records = csv.GetRecords<MeterReading>().ToList();

                    List<Account> accounts = await _db.Accounts.ToListAsync();

                    foreach (var record in records)
                    {
                        if(accounts.Any(x => x.AccountId == record.AccountId))
                        {
                            record.Id = Guid.NewGuid();
                            ValidRecords.Add(record);
                        }
                        else
                        {
                            InValidRecords.Add(record);
                        }
                        
                        _db.MeterReadings.Add(record);
                    }


                    _logger.LogInformation($"Valid Records Found: {ValidRecords?.Count}");
                    _logger.LogInformation($"Invalid Records Found: {InValidRecords?.Count}");
                    _logger.LogInformation($"Begining Upsert");

                    for (int i = 0; i < ValidRecords?.Count; i++) 
                    {
                        MeterReading? record = ValidRecords[i];
                        _db.MeterReadings.Add(record);
                    }

                    await _db.MeterReadings.AddRangeAsync(ValidRecords, cancellationToken);
                    await _db.SaveChangesAsync(cancellationToken);
                }
            }

            return Ok("Meter readings uploaded successfully.");
        }
    }
}
