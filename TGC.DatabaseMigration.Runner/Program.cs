using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;
using TGC.DatabaseMigration.DBUpWrapper;
using TGC.DatabaseMigration.DBUpWrapper.Interfaces;
using TGC.DatabaseMigration.EvolveWrapper;
using TGC.DatabaseMigration.Shared;

StartApplicationEvolve();

static void StartApplicationDBUp()
{
    var iocContainer = new IoCContainer();

    var serviceProvider = iocContainer.BuildServiceProvider();

    var migrationBuilder = serviceProvider.GetService<IMigrationBuilder>();
    var migrationRunner = serviceProvider.GetService<IMigrationRunner>();

    var upgradeEngine = migrationBuilder.BuildTrackedUpgradeEngine("R2022.01.1");

    migrationRunner.ListAppliedMigrations(upgradeEngine);
    migrationRunner.ListApplicableMigrations(upgradeEngine);

    migrationRunner.Run(upgradeEngine);
}

static void StartApplicationEvolve()
{
    var iocContainer = new IoCContainer();

    var serviceProvider = iocContainer.BuildServiceProvider();

    var migrationRunner = new EvolveMigrationRunner(serviceProvider.GetService<ILogger>(), serviceProvider.GetService<IOptions<AppSettings>>());
    migrationRunner.Run();


}