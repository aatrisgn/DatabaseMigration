using TGC.DatabaseMigration.Runner.Interfaces;
using TGC.DatabaseMigration.Runner.Models;
using TGC.DatabaseMigration.Runner.Models.Enums;

namespace TGC.DatabaseMigration.Runner
{
    public class AppSettings : IAppSettings
    {
        public string ConnectionString { get; set; }
        public bool EnsureDatabase { get; set; }
        public MigrationSetting TrackedMigrations { get; set; }
        public MigrationSetting IdempotentMigrations { get; set; }
    }
}
