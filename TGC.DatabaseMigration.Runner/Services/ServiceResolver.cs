using Microsoft.Extensions.DependencyInjection;
using TGC.DatabaseMigration.DBUpWrapper;
using TGC.DatabaseMigration.Runner.CmdOptions;

namespace TGC.DatabaseMigration.Runner.Services;
internal class ServiceResolver : IServiceResolver
{
    public ServiceProvider ResolveServiceProvider()
    {
        var iocContainer = new IoCContainer();
        return iocContainer.BuildServiceProvider();
    }

    public int ApplyDatabaseChanges(BaseCmdOptions baseCmdOptions)
    {
        return 0;
    }
}