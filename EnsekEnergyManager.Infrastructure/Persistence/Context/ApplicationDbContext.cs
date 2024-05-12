using EnsekEnergyManager.Classes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnsekEnergyManager.Infrastructure.Persistence.Context
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IConfiguration _configuration;
        public ApplicationDbContext(DbContextOptions options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }

        public DbSet<Account> Accounts => Set<Account>();
        public DbSet<MeterReading> MeterReadings => Set<MeterReading>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            string connectionString = _configuration.GetConnectionString("DefaultConnection");


           optionsBuilder.UseSqlServer("Data Source=Main-01\\ENTEKSERVER;Initial Catalog=ENSEKDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True");
        }
    }
}
