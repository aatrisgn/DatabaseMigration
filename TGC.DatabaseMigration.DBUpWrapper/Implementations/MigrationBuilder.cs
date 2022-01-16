using DbUp;
using DbUp.Engine;
using DbUp.Helpers;
using Microsoft.Extensions.Options;
using Serilog;
using System.Collections.ObjectModel;
using System.Reflection;
using TGC.DatabaseMigration.DBUpWrapper.Extensions;
using TGC.DatabaseMigration.DBUpWrapper.Interfaces;
using TGC.DatabaseMigration.Shared;

namespace TGC.DatabaseMigration.DBUpWrapper.Implementations
{
    class MigrationBuilder : IMigrationBuilder
    {
        private readonly AppSettings _appSettings;
        private readonly ILogger _logger;
        public MigrationBuilder(IOptions<AppSettings> appSettings, ILogger logger)
        {
            _appSettings = appSettings?.Value;
            _logger = logger;
        }

        public UpgradeEngine BuildTrackedRollbackEngine(string releaseNumber)
        {
            var directoryNames = DetermineRelevantRollbackReleases(releaseNumber);

            var some = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            some = Path.Combine(some, "Migrations");

            var upgradeEngine = DeployChanges.To
                    .SqlDatabase(_appSettings.ConnectionString)
                    .WithScriptsEmbeddedInAssembly(Assembly.LoadFrom("TGC.DatabaseMigration.Migrations.dll"), s => PartOfRelease(s, directoryNames) && s.Contains("ROLLBACK"))
                    .JournalTo(new NullJournal())
                    .SetTransactionlevel(_appSettings.TrackedMigrations.TransactionLevel)
                    .LogToSerilog(new CustomLogging(_logger))
                    .Build();

            return upgradeEngine;
        }

        public UpgradeEngine BuildTrackedUpgradeEngine(string releaseNumber)
        {
            var directoryNames = DetermineRelevantReleases(releaseNumber);

            var some = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            some = Path.Combine(some, "Migrations");

            var upgradeEngine = DeployChanges.To
                    .SqlDatabase(_appSettings.ConnectionString)
                    .WithScriptsEmbeddedInAssembly(Assembly.LoadFrom("TGC.DatabaseMigration.Migrations.dll"), s => PartOfRelease(s, directoryNames) && s.Contains("ROLLBACK") == false)
                    .JournalToCustomSqlTable(_appSettings.TrackedMigrations.SchemaName, _appSettings.TrackedMigrations.MigrationTable)
                    .SetTransactionlevel(_appSettings.TrackedMigrations.TransactionLevel)
                    .LogToSerilog(new CustomLogging(_logger))
                    .Build();
            
            return upgradeEngine;
        }

        public UpgradeEngine BuildIdempotentRollbackEngine()
        {
            return DeployChanges.To
              .SqlDatabase(_appSettings.ConnectionString)
              .WithScriptsEmbeddedInAssembly(
                  Assembly.GetExecutingAssembly(),
                  s => s.Contains("Migrations.Idempotent") && s.Contains("ROLLBACK"))
              .JournalTo(new NullJournal())
              .SetTransactionlevel(_appSettings.IdempotentMigrations.TransactionLevel)
              .Build();
        }

        public UpgradeEngine BuildIdempotentUpgradeEngine()
        {
            return DeployChanges.To
              .SqlDatabase(_appSettings.ConnectionString)
              .WithScriptsEmbeddedInAssembly(
                  Assembly.GetExecutingAssembly(),
                  s => s.Contains("Migrations.Idempotent"))
              .JournalTo(new NullJournal())
              .SetTransactionlevel(_appSettings.IdempotentMigrations.TransactionLevel)
              .Build();
        }

        private string[] DetermineRelevantReleases(string releaseNumber)
        {
            var relevantReleaseDirectories = new Collection<string>();

            var migrationDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Migrations");
            var releaseDirectories = Directory.GetDirectories(migrationDirectory).Where(d => d.Contains("Idempotent") == false);

            if (RelevantReleaseDirectoryExists(releaseNumber, releaseDirectories))
            {
                releaseDirectories = releaseDirectories.OrderBy(d => d);
                foreach (var releaseDirectory in releaseDirectories)
                {
                    relevantReleaseDirectories.Add(releaseDirectory.Replace(".", "._"));
                    if (releaseDirectory.Contains(releaseNumber))
                    {
                        break;
                    }
                }
            }

            return relevantReleaseDirectories.Select(d => new DirectoryInfo(d).Name).ToArray();
        }

        private string[] DetermineRelevantRollbackReleases(string releaseNumber)
        {
            var relevantReleaseDirectories = new Collection<string>();

            var migrationDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Migrations");
            var releaseDirectories = Directory.GetDirectories(migrationDirectory).Where(d => d.Contains("Idempotent") == false);

            if (RelevantReleaseDirectoryExists(releaseNumber, releaseDirectories))
            {
                releaseDirectories = releaseDirectories.OrderByDescending(d => d);
                foreach (var releaseDirectory in releaseDirectories)
                {
                    relevantReleaseDirectories.Add(releaseDirectory.Replace(".","._"));
                    if (releaseDirectory.Contains(releaseNumber))
                    {
                        break;
                    }
                }
            }

            return relevantReleaseDirectories.Select(d => new DirectoryInfo(d).Name).ToArray();
        }

        private bool RelevantReleaseDirectoryExists(string releaseNumber, IEnumerable<string> directories)
        {
            return directories.Any(d => d.Contains(releaseNumber));
        }

        private bool PartOfRelease(string migrationScript, string[] relevantReleases)
        {
            foreach (string needle in relevantReleases)
            {
                if (migrationScript.Contains(needle))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
