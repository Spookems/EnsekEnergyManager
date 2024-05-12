using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnsekEnergyManager.Infrastructure.Persistence.Context
{
    public interface IDatabaseConfig
    {
        string ConnectionString { get; set; }
    }

    public class DatabaseConfig : IDatabaseConfig
    {
        public DatabaseConfig()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            ConnectionString = "Data Source=Main-01\\ENTEKSERVER;Initial Catalog=ENSEKDB;Integrated Security=True;Trust Server Certificate=True";
            
            // ConnectionString = configuration.GetSection("Database").GetValue<string>("ConnectionString");
        }

        public string ConnectionString { get; set; }

    }
}
