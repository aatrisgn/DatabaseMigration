namespace TGC.DatabaseMigration.Runner.Interfaces
{
    public interface IAppSettings
    {
        string ConnectionString { get; set; }
        bool EnsureDatabase { get; set; }
    }
}
