using DbUp.Engine.Output;
using Serilog;

namespace TGC.DatabaseMigration.DBUpWrapper;
public class CustomLogging : IUpgradeLog
{
    private readonly ILogger _logger;
    public CustomLogging(ILogger logger)
    {
        _logger = logger;
    }

    public void WriteError(string format, params object[] args)
    {
        _logger.Error(format, args);
    }

    public void WriteInformation(string format, params object[] args)
    {
        _logger.Information(format, args);
    }

    public void WriteWarning(string format, params object[] args)
    {
        _logger.Warning(format, args);
    }
}
