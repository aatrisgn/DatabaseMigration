using DbUp;
using DbUp.Engine;
using DbUp.Helpers;
using Microsoft.Extensions.Options;
using Serilog;
using TGC.DatabaseMigration.DBUpWrapper.Interfaces;
using TGC.DatabaseMigration.Shared;

namespace TGC.DatabaseMigration.DBUpWrapper.Implementations
{
    class MigrationRunner : IMigrationRunner
    {
        private readonly AppSettings _appSettings;
        private readonly ILogger _logger;
        public MigrationRunner(IOptions<AppSettings> appSettings, ILogger logger)
        {
            _appSettings = appSettings?.Value;
            _logger = logger;
        }

        public void Run(UpgradeEngine upgradeEngine)
        {
            if (_appSettings.EnsureDatabase)
            {
                EnsureDatabase.For.SqlDatabase(_appSettings.ConnectionString);
            }

            var some = upgradeEngine.GetDiscoveredScripts();

            if (upgradeEngine.IsUpgradeRequired() == false)
            {
                _logger.Information("No relevant migrations was found.");
                return;
            }

            var result = upgradeEngine.PerformUpgrade();

            if (!result.Successful)
            {
                _logger.Error(result.Error.Message);
                _logger.Error(result.Error.ToString());
            } else
            {
                _logger.Information("Migrations applied succesfully!");
            }
        }

        public bool TestConnection(UpgradeEngine upgradeEngine)
        {
            var connectionSuccesful = upgradeEngine.TryConnect(out var errorMessage);
            if (connectionSuccesful)
            {
                _logger.Information("Connected succesfully");
            }else
            {
                _logger.Error("Was not able to connect to database:");
                _logger.Error(errorMessage);
            }
            return connectionSuccesful;
        }

        public void GenerateHtmlReport(UpgradeEngine upgradeEngine, string path)
        {
            upgradeEngine.GenerateUpgradeHtmlReport(path);
        }

        public void ListAppliedMigrations(UpgradeEngine upgradeEngine)
        {
            var executedScripts = upgradeEngine.GetExecutedScripts();

            _logger.Information("The following migrations has been applied:");

            if(executedScripts.Count != 0)
            {
                foreach (var executedScript in executedScripts)
                {
                    _logger.Information(executedScript);
                }
            } else
            {
                _logger.Information("None.");
            }
        }

        public void ListApplicableMigrations(UpgradeEngine upgradeEngine)
        {
            var executedScripts = upgradeEngine.GetScriptsToExecute();

            _logger.Information("The following migrations is applicable");

            if (executedScripts.Count != 0)
            {
                foreach (var executedScript in executedScripts)
                {
                    _logger.Information(executedScript.Name);
                }
            }
            else
            {
                _logger.Information("None.");
            }
        }
    }
}
