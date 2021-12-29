﻿using DbUp;
using DbUp.Engine;
using DbUp.Helpers;
using Microsoft.Extensions.Options;
using Serilog;
using System.Collections.ObjectModel;
using System.Reflection;
using TGC.DatabaseMigration.Runner.Extensions;
using TGC.DatabaseMigration.Runner.Interfaces;

namespace TGC.DatabaseMigration.Runner.Implementations
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

            return DeployChanges.To
                    .SqlDatabase(_appSettings.ConnectionString)
                    .WithScriptsEmbeddedInAssembly(
                        Assembly.GetExecutingAssembly(),
                        s => PartOfRelease(s, directoryNames) && s.Contains("ROLLBACK"))
                    .JournalToSqlTable(_appSettings.TrackedMigrations.SchemaName, _appSettings.TrackedMigrations.MigrationTable)
                    .SetTransactionlevel(_appSettings.TrackedMigrations.TransactionLevel)
                    .LogToConsole()
                    .Build();
        }

        public UpgradeEngine BuildTrackedUpgradeEngine(string releaseNumber)
        {
            var directoryNames = DetermineRelevantReleases(releaseNumber);

            return DeployChanges.To
                    .SqlDatabase(_appSettings.ConnectionString)
                    .WithScriptsEmbeddedInAssembly(
                        Assembly.GetExecutingAssembly(),
                        s => PartOfRelease(s, directoryNames) && s.Contains("ROLLBACK") == false)
                    .JournalToSqlTable(_appSettings.TrackedMigrations.SchemaName, _appSettings.TrackedMigrations.MigrationTable)
                    .SetTransactionlevel(_appSettings.TrackedMigrations.TransactionLevel)
                    .LogToConsole()
                    .Build();
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
                    relevantReleaseDirectories.Add(releaseDirectory);
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
                    relevantReleaseDirectories.Add(releaseDirectory);
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
