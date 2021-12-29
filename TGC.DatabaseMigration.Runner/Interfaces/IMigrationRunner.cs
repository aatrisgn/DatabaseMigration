using DbUp.Engine;

namespace TGC.DatabaseMigration.Runner.Interfaces
{
    interface IMigrationRunner
    {
        void Run(UpgradeEngine upgradeEngine);
        bool TestConnection(UpgradeEngine upgradeEngine);
        void ListAppliedMigrations(UpgradeEngine upgradeEngine);
        void ListApplicableMigrations(UpgradeEngine upgradeEngine);
        void GenerateHtmlReport(UpgradeEngine upgradeEngine, string path);
    }
}
