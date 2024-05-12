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
using EnsekEnergyManager.Infrastructure.Persistence.Init;
using Microsoft.EntityFrameworkCore;
using CsvHelper;
using Microsoft.SqlServer.Server;
using System.Globalization;

namespace EnsekEnergyManager.Infrastructure.Seeders
{
    public class MeterReadingObject
    {
        public Guid Id { get; set; }
        public int AccountId { get; set; }
        public DateTime? MeterReadingDateTime { get; set; }
        public string? MeterReadValue { get; set; } = string.Empty;
    }

    internal class MeterSeeder : ICustomSeeder
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
            string filePath = $"C:/Users/Dan/source/repos/Spookems/EnsekEnergyManager/EnsekEnergyManager.Infrastructure/Seeders/Csv/Meter_Reading.csv"; // Path to your CSV file
            List<MeterReading> meterReadings = await ParseMeterReadingsAsync(filePath);

            if (meterReadings.Count > 0)
            {
                _logger.LogInformation("Seeding meter readings.");
                await _db.MeterReadings.AddRangeAsync(meterReadings, cancellationToken);
                await _db.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Seeded meter readings.");
            }
        }

        public async Task<List<MeterReading>> ParseMeterReadingsAsync(string filePath)
        {
            List<MeterReading> meterReadings = new List<MeterReading>();

            using (var reader = new StreamReader(filePath))
            {
                reader.ReadLine();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    // Assuming the CSV columns are in the order: AccountId, MeterReadingDateTime, MeterReadValue
                    if (values.Length >= 3)
                    {
                        int accountId;
                        if (int.TryParse(values[0], out accountId))
                        {
                            var associatedAccount = await _db.Accounts.Where(x => x.AccountId == accountId).FirstOrDefaultAsync();

                            CultureInfo provider = CultureInfo.InvariantCulture;
                            DateTime meterReadingDateTime = DateTime.ParseExact(values[1], "M/d/yy H:mm", System.Globalization.CultureInfo.InvariantCulture);

                            if (associatedAccount != null)
                            {
                                string meterReadValue = values[2];

                                MeterReading meterReading = new MeterReading
                                {
                                    AccountId = accountId,
                                    MeterReadingDateTime = meterReadingDateTime,
                                    MeterReadValue = meterReadValue
                                };

                                meterReadings.Add(meterReading);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid AccountId format.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid line format: " + line);
                    }
                }
            }



            return meterReadings;
        }
    }
}
