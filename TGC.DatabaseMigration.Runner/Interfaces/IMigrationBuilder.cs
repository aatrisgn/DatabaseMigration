using DbUp.Engine;

namespace TGC.DatabaseMigration.Runner.Interfaces
{
    interface IMigrationBuilder
    {
        UpgradeEngine BuildTrackedUpgradeEngine(string releaseNumber);
        UpgradeEngine BuildIdempotentUpgradeEngine();
    }
}
