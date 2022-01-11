using DbUp.Engine;

namespace TGC.DatabaseMigration.DBUpWrapper.Interfaces
{
    public interface IMigrationBuilder
    {
        UpgradeEngine BuildTrackedUpgradeEngine(string releaseNumber);
        UpgradeEngine BuildIdempotentUpgradeEngine();
    }
}
