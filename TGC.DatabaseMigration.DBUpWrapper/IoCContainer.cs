using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using TGC.DatabaseMigration.DBUpWrapper.Implementations;
using TGC.DatabaseMigration.DBUpWrapper.Interfaces;
using TGC.DatabaseMigration.Shared;

namespace TGC.DatabaseMigration.DBUpWrapper
{
    public class IoCContainer : ServiceCollection
    {
        public IoCContainer()
        {
            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("AppSettings.json", optional: false)
            .Build();

            var logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            this.AddOptions();

            var some = configuration.GetSection("App");
            this.Configure<AppSettings>(some);

            this.AddSingleton<ILogger>(logger);
            this.AddScoped<IMigrationBuilder, MigrationBuilder>();
            this.AddScoped<IMigrationRunner, MigrationRunner>();
        }
    }
}
