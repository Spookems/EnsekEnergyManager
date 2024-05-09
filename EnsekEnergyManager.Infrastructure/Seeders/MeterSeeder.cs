using EnsekEnergyManager.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnsekEnergyManager.Infrastructure.Persistence.Context;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace EnsekEnergyManager.Infrastructure.Seeders
{
    public class MeterReadingObject
    {
        public Guid Id { get; set; }
        public Account? Account { get; set; }
        public DateTime? MeterReadingDateTime { get; set; }
        public string? MeterReadValue { get; set; } = string.Empty;
    }

    internal class MeterSeeder
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<MeterSeeder> _logger;

        public MeterSeeder(ILogger<MeterSeeder> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public async Task InitializeAsync(CancellationToken cancellationToken)
        {
            string? authorPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            string authorsData = await File.ReadAllTextAsync(authorPath + "/Seeders/authors.json", cancellationToken); // needs changing for csv
            List<MeterReadingObject> authors = _serializerService.Deserialize<List<MeterReadingObject>>(authorsData);

            if (authors != null)
            {
                Microsoft.EntityFrameworkCore.DbSet<MeterReading> existingType = _db.MeterReadings;
                //IEnumerable<MeterReading> missing = authors.Where(lt => !existingType.Any(l => l.MeterReadValue != null && l.MeterReadValue == lt.MeterReadValue));

                //if (missing.Any())
                //{
                    _logger.LogInformation("Started to Seed Authors.");
                    foreach (MeterReadingObject author in authors)
                    {
                        MeterReading newAuthor = new MeterReading(author.MeterReadValue, null, null);
                        await _db.MeterReadings.AddAsync(newAuthor, cancellationToken);
                    }

                    await _db.SaveChangesAsync(cancellationToken);
                    _logger.LogInformation("Seeded Authors.");
                //}
            }
        }
    }
}
