using CommandLine;

namespace TGC.DatabaseMigration.Runner.CmdOptions;
internal class BaseCmdOptions
{
    [Option('r', "release", Required = true, HelpText = "Release to apply/rollback")]
    public string Release { get; set; }
}
