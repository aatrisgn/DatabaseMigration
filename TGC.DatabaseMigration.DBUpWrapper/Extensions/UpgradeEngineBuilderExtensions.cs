using DbUp.Builder;
using DbUp.Engine.Output;
using DbUp.SqlServer;
using TGC.DatabaseMigration.Shared.Models.Enums;

namespace TGC.DatabaseMigration.DBUpWrapper.Extensions
{
    public static class UpgradeEngineBuilderExtensions
    {
        public static UpgradeEngineBuilder SetTransactionlevel(this UpgradeEngineBuilder upgradeEngineBuilder, TransactionLevelType transactionLevelType)
        {
            switch (transactionLevelType)
            {
                case TransactionLevelType.SingleTransaction:
                    upgradeEngineBuilder.WithTransactionPerScript();
                    break;
                case TransactionLevelType.TransactionScript:
                    upgradeEngineBuilder.WithTransaction();
                    break;
                default:
                    upgradeEngineBuilder.WithoutTransaction();
                    break;
            }

            return upgradeEngineBuilder;
        }

        public static UpgradeEngineBuilder JournalToCustomSqlTable(this UpgradeEngineBuilder builder, string schema, string table)
        {
            builder.Configure(c => c.Journal = new CustomJournal(() => c.ConnectionManager, () => c.Log, schema, table));
            return builder;
        }

        public static UpgradeEngineBuilder LogToSerilog(this UpgradeEngineBuilder builder, CustomLogging log)
        {
            builder.Configure(c => c.AddLog(log));
            return builder;
        }
    }
}
