using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnsekEnergyManager.Infrastructure.Persistence.Context
{
    public abstract class BaseDbContext 
    {
        ////private readonly DatabaseSettings _dbSettings;

        ////protected BaseDbContext(DbContextOptions options, IOptions<DatabaseSettings> dbSettings)
        ////    : base(options)
        ////{
        ////    _dbSettings = dbSettings.Value;
        ////}

        ////protected override void OnModelCreating(ModelBuilder modelBuilder)
        ////{
        ////    base.OnModelCreating(modelBuilder);

        ////    modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        ////}
    }
}
