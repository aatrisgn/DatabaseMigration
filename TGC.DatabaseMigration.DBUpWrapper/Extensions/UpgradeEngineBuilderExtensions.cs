using DbUp.Builder;
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
    }
}
