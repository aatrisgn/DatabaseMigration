using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using TGC.DatabaseMigration.Runner.Implementations;
using TGC.DatabaseMigration.Runner.Interfaces;

namespace TGC.DatabaseMigration.Runner
{
    class IoCContainer : ServiceCollection
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
