using Microsoft.Extensions.Options;
using Serilog;
using System.Data.SqlClient;
using TGC.DatabaseMigration.Shared;

namespace TGC.DatabaseMigration.EvolveWrapper
{
    public class EvolveMigrationRunner
    {
        private readonly ILogger _logger;
        private readonly AppSettings _appSettings;
        public EvolveMigrationRunner(ILogger logger, IOptions<AppSettings> appSettings)
        {
            _logger = logger;
            _appSettings = appSettings?.Value;
        }

        public void Run()
        {
            try
            {
                var cnx = new SqlConnection(_appSettings.ConnectionString);
                var evolve = new Evolve.Evolve(cnx, msg => _logger.Information(msg))
                {
                    Locations = new[] { "Migrations" },
                    IsEraseDisabled = true,
                };

                evolve.Migrate();
            }
            catch (Exception ex)
            {
                _logger.Fatal("Database migration failed.", ex);
                throw;
            }
        }
    }
}