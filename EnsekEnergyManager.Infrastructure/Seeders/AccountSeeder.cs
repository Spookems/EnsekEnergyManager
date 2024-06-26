﻿using EnsekEnergyManager.Infrastructure.Persistence.Context;
using EnsekEnergyManager.Infrastructure.Persistence.Init;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using EnsekEnergyManager.Classes;
using System.ComponentModel.DataAnnotations;
using System.IO;
using OfficeOpenXml;
using System.Globalization;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore;

namespace EnsekEnergyManager.Infrastructure.Seeders
{
    public class AccountSeeder : ICustomSeeder
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<AccountSeeder> _logger;

        public class AccountObject
        {
            public int AccountId { get; set; }
            public string? FirstName { get; set; }
            public string? LastName { get; set; } 
        }

        public AccountSeeder(ILogger<AccountSeeder> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        private async Task<List<AccountObject>> LoadAccountsFromCsvAsync(string csvPath, CancellationToken cancellationToken)
        {
            CsvMapperHelper csvMappingHelper = new CsvMapperHelper(_logger);

            List<AccountObject> accounts = await GetRecordsFromCSVAsync($"C:/Users/Dan/source/repos/Spookems/EnsekEnergyManager/EnsekEnergyManager.Infrastructure/Seeders/Csv/Test_Accounts.csv", cancellationToken);
            return accounts;
        }

        static List<AccountObject> ReadCsv(string filePath)
        {
            List<AccountObject> accounts = new List<AccountObject>();

            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                // Skip the header line
                reader.ReadLine();
                while ((line = reader.ReadLine()) != null)
                {
                    string[] parts = line.Split(',');

                    int accountId = int.Parse(parts[0]);
                    string firstName = parts[1];
                    string lastName = parts[2];

                    accounts.Add(new AccountObject() { AccountId = accountId,FirstName = firstName, LastName = lastName });
                }
            }

            return accounts;
        }

        public async Task<List<AccountObject>> GetRecordsFromCSVAsync(string filePath, CancellationToken cancellationToken)
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

                    await foreach (var record in csv.GetRecordsAsync<AccountObject>(cancellationToken))
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

        public async Task InitializeAsync(CancellationToken cancellationToken)
        {
            string? csvPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (csvPath == null)
            {
                _logger.LogError("Failed to get movie path.");
                return;
            }

            List<AccountObject> accounts = await LoadAccountsFromCsvAsync(csvPath, cancellationToken);
            if (accounts.Count == 0)
            {
                _logger.LogWarning("No movies found to process.");
                return;
            }
            ApplicationDbContext _db = new();
            List<Account> addedAccounts = await SeedAccountsAsync(accounts, cancellationToken, _db);
            _logger.LogInformation($"Accounts Added { addedAccounts?.Count }.");
        }

        public async Task<List<Account>> SeedAccountsAsync(IEnumerable<AccountObject> movies, CancellationToken cancellationToken, ApplicationDbContext _db)
        {
            
            List<Account> missingAccountsToUpdate = [];
            HashSet<int> existingTitles = _db.Accounts.Where(x => x.AccountId != null).Select(x => x.AccountId).ToHashSet();
            IEnumerable<AccountObject> missingMovies = movies.Where(m => !existingTitles.Contains(m.AccountId));

            if (missingMovies.Any())
            {
                _logger.LogInformation("Started to Seed movies.");
                foreach (AccountObject AccountObj in missingMovies)
                {
                    Account account = new Account() { FirstName = AccountObj?.FirstName, LastName = AccountObj?.LastName };
                    account.Update(account.AccountId, account.FirstName, account.LastName);
                    missingAccountsToUpdate.Add(account);
                }

                if (missingAccountsToUpdate.Count > 0)
                {
                    _logger.LogInformation("Seeding accounts.");
                    await _db.Accounts.AddRangeAsync(missingAccountsToUpdate, cancellationToken);
                    await _db.SaveChangesAsync(cancellationToken);
                    _logger.LogInformation("Seeded accounts.");
                } 
            }

            return missingAccountsToUpdate;
        }
    }
}
