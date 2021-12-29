using TGC.DatabaseMigration.Runner.Models.Enums;

namespace TGC.DatabaseMigration.Runner.Models
{
    public class MigrationSetting
    {
        public string SchemaName {get;set;}
        public string MigrationTable { get; set; }
        public TransactionLevelType TransactionLevel { get; set; }
    }
}
