namespace TGC.DatabaseMigration.Shared
{
    public interface IAppSettings
    {
        string ConnectionString { get; set; }
        bool EnsureDatabase { get; set; }
    }
}
