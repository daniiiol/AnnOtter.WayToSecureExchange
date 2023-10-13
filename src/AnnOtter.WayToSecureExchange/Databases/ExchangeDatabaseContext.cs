using AnnOtter.WayToSecureExchange.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace AnnOtter.WayToSecureExchange.Databases
{
    /// <summary>
    /// Configuration of the Main Database.
    /// This software supports local SQLite and external PostgreSQL installations.
    /// 
    /// External database installations could be configured over environment variables or the local 'appsettings.json' file.
    /// </summary>
    public class ExchangeDatabaseContext : DbContext
    {
        /// <summary>
        /// Database path if SQLite is configured.
        /// </summary>
        public string DbPath { get; set; }

        /// <summary>
        /// Database entity "secrets" to store all secret objects in the database.
        /// </summary>
        public DbSet<SecretEntity> Secrets { get; set; }

        /// <summary>
        /// Ctor of the database context. It sets the default SQLite path and executes Database.Migrate().
        /// </summary>
        public ExchangeDatabaseContext()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);

            DbPath = Path.Join(path, "ao_exchangedatabase.db");
            Database.Migrate();
        }

        /// <summary>
        /// checks the current database settings. If an environment variable exists, it gets priority 1. The settings in appsettings.json have priority 2. If no settings are found, a SQLite database is activated and used.
        /// </summary>
        /// <param name="options">Passing the DbContextOptionsBuilder object for activating the supported database type.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

            // For usage in a Env-Configuration-Setup (e.g. Azure AppService)
            var envConString = Environment.GetEnvironmentVariable("POSTGRESQLCONNSTR_DatabaseConnection_AO_ExchangeDatabase");
            // For usage in a classic mode (appsettings.json configuration)
            var connectionString = !string.IsNullOrEmpty(envConString) ? envConString : configuration.GetConnectionString("DatabaseConnection_AO_ExchangeDatabase");

            
            if (string.IsNullOrEmpty(connectionString)) // For a local usage without external database
            {
                optionsBuilder.UseSqlite($"Data Source={DbPath}");
            }
            else
            {
                optionsBuilder.UseNpgsql(connectionString);
            }
        }
    }
}
