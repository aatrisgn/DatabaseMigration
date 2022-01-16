using CommandLine;
using DbUp.Engine;
using Microsoft.Extensions.DependencyInjection;
using TGC.DatabaseMigration.DBUpWrapper;
using TGC.DatabaseMigration.DBUpWrapper.Interfaces;
using TGC.DatabaseMigration.Runner.CmdOptions;

/*
 * Example: .exe Apply -R/--release "R2022.02.0"
 * Example: .exe Rollback -R/--release "R2022.02.0"
 * Example: .exe CreateMigration -M/--MigrationName "AZ-NC-12345"
 * 
*/

ResolveCmdArguments(args);

void ResolveCmdArguments(string[] cmdArgs)
{
    var some = CommandLine.Parser.Default.ParseArguments<ApplyCmdOptions, RollbackCmdOptions, CreateMigrationCmdOptions>(cmdArgs)
    .MapResult(
      (ApplyCmdOptions opts) => ApplyMigration(opts),
      (RollbackCmdOptions opts) => RollbackMigration(opts),
      (CreateMigrationCmdOptions opts) => CreateMigration(opts),
      errs => 1);
}

int ApplyMigration(ApplyCmdOptions applyCmdOptions)
{
    var serviceProvider = ResolveServiceProvider();

    var migrationBuilder = serviceProvider.GetService<IMigrationBuilder>();
    var migrationRunner = serviceProvider.GetService<IMigrationRunner>();

    var upgradeEngine = migrationBuilder.BuildTrackedUpgradeEngine(applyCmdOptions.Release);

    ExecuteMigrationRunner(upgradeEngine, migrationRunner);

    return 1;
}

int RollbackMigration(RollbackCmdOptions rollbackCmdOptions)
{
    var serviceProvider = ResolveServiceProvider();

    var migrationBuilder = serviceProvider.GetService<IMigrationBuilder>();
    var migrationRunner = serviceProvider.GetService<IMigrationRunner>();

    var upgradeEngineRollback = migrationBuilder.BuildTrackedRollbackEngine(rollbackCmdOptions.Release);

    ExecuteMigrationRunner(upgradeEngineRollback, migrationRunner);

    return 1;
}

int CreateMigration(CreateMigrationCmdOptions createMigrationCmdOptions)
{
    var serviceProvider = ResolveServiceProvider();

    throw new NotImplementedException();
}

ServiceProvider ResolveServiceProvider()
{
    var iocContainer = new IoCContainer();
    return iocContainer.BuildServiceProvider();
}

void ExecuteMigrationRunner(UpgradeEngine upgradeEngine, IMigrationRunner migrationRunner)
{
    migrationRunner.ListAppliedMigrations(upgradeEngine);
    migrationRunner.ListApplicableMigrations(upgradeEngine);
    migrationRunner.Run(upgradeEngine);
}

//static void StartApplicationEvolve()
//{
//    var iocContainer = new IoCContainer();

//    var serviceProvider = iocContainer.BuildServiceProvider();

//    var migrationRunner = new EvolveMigrationRunner(serviceProvider.GetService<ILogger>(), serviceProvider.GetService<IOptions<AppSettings>>());
//    migrationRunner.Run();


//}