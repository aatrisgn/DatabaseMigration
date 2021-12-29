using DbUp.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.DatabaseMigration.Runner.Models.Enums;

namespace TGC.DatabaseMigration.Runner.Extensions
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
