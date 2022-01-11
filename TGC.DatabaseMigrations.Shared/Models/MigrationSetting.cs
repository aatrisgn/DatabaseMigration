using TGC.DatabaseMigration.Shared.Models.Enums;

namespace TGC.DatabaseMigration.Shared.Models
{
    public class MigrationSetting
    {
        public string SchemaName {get;set;}
        public string MigrationTable { get; set; }
        public TransactionLevelType TransactionLevel { get; set; }
    }
}
