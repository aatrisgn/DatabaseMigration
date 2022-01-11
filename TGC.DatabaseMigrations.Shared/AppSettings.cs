using TGC.DatabaseMigration.Shared;
using TGC.DatabaseMigration.Shared.Models;
using TGC.DatabaseMigration.Shared.Models.Enums;

namespace TGC.DatabaseMigration.Shared
{
    public class AppSettings : IAppSettings
    {
        public string ConnectionString { get; set; }
        public bool EnsureDatabase { get; set; }
        public MigrationSetting TrackedMigrations { get; set; }
        public MigrationSetting IdempotentMigrations { get; set; }
    }
}
