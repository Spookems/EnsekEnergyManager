using CsvHelper.Configuration;
using CsvHelper;
using Microsoft.Extensions.Logging;
using System.Globalization;
using static EnsekEnergyManager.Infrastructure.Seeders.AccountSeeder;

namespace EnsekEnergyManager.Infrastructure.Seeders
{
    internal class CsvMapperHelper
    {
        private readonly ILogger<AccountSeeder> _logger;
        public CsvMapperHelper(ILogger<AccountSeeder> logger)
        {
            _logger = logger;
        }

        public async Task<List<AccountObject>> GetMoviesFromCsvAsync(string filePath, CancellationToken cancellationToken)
        {
            try
            {
                CsvConfiguration config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ",",
                    HasHeaderRecord = true,
                    IgnoreBlankLines = true,
                    TrimOptions = TrimOptions.Trim,
                    MissingFieldFound = null,
                    ReadingExceptionOccurred = context =>
                    {
                        return false;
                    },
                };

                using (StreamReader reader = new StreamReader(filePath))
                using (CsvReader csv = new CsvReader(reader, config))
                {
                    List<AccountObject> records = new List<AccountObject>();

                    await foreach (AccountObject record in csv.GetRecordsAsync<AccountObject>(cancellationToken))
                    {
                        records.Add(record);
                    }

                    return records;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occured while trying to read csv file. {ex.Message}");
                throw;
            }
        }
    }
}
