using DbUp.Engine;

namespace TGC.DatabaseMigration.DBUpWrapper.Interfaces
{
    public interface IMigrationRunner
    {
        void Run(UpgradeEngine upgradeEngine);
        bool TestConnection(UpgradeEngine upgradeEngine);
        void ListAppliedMigrations(UpgradeEngine upgradeEngine);
        void ListApplicableMigrations(UpgradeEngine upgradeEngine);
        void GenerateHtmlReport(UpgradeEngine upgradeEngine, string path);
    }
}
