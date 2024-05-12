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
using Microsoft.Testing.Platform.Logging;

namespace EnsekEnergyManager.Infrastructure.Seeders
{
    public class MeterReadingObject
    {
        public Guid Id { get; set; }
        public int AccountId { get; set; }
        public DateTime? MeterReadingDateTime { get; set; }
        public string? MeterReadValue { get; set; } = string.Empty;
    }

    public class MeterSeeder : ICustomSeeder
    {
        private readonly ApplicationDbContext _db;
        private readonly Microsoft.Extensions.Logging.ILogger<MeterSeeder> _logger;

        public MeterSeeder(Microsoft.Extensions.Logging.ILogger<MeterSeeder> logger, ApplicationDbContext db)
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

            if (_db == null)
            {
                ApplicationDbContext _db = new();
            }
            List<MeterReading> meterReadings = new List<MeterReading>();
            if (File.Exists(filePath))
            {
                // Check if the file is not empty
                if (new FileInfo(filePath).Length > 0)
                {
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
                                    _logger.LogInformation("Invalid AccountId format.");
                                }
                            }
                            else
                            {
                                _logger.LogInformation("Invalid line format: " + line);
                            }
                        }
                    }
                }
                else
                {
                    _logger.LogInformation("File is empty: " + filePath);
                }
            }
            else
            {
                _logger.LogInformation("File does not exist: " + filePath);
            }

            return meterReadings;
        }
    }
}
