using Microsoft.Extensions.DependencyInjection;
using TGC.DatabaseMigration.Runner;
using TGC.DatabaseMigration.Runner.Interfaces;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;

var rootCommand = new RootCommand
    {
        new Command("upgrade")
        {
            new Option<string>(
            "--release",
            description: "An option which release to work with."),
        },
        new Command("rollback")
        {
            new Option<string>(
            "--release",
            description: "An option which release to work with."),
        }
    };

rootCommand.Description = "My sample app";

// Note that the parameters of the handler method are matched according to the names of the options
rootCommand.Handler = CommandHandler.Create(() =>
{
    Console.WriteLine("Upgrade command");
});

StartApplication();

// Parse the incoming args and invoke the handler
var some = rootCommand.InvokeAsync(args).Result;

static void StartApplication()
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