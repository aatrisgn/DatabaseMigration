using CommandLine;

namespace TGC.DatabaseMigration.Runner.CmdOptions;

[Verb("CreateMigration", HelpText = "Create a new migration and rollback script based on template")]
class CreateMigrationCmdOptions : BaseCmdOptions
{
    [Option('m', "MigrationId", Required = true, HelpText = "Unique ID for migration")]
    public string MigrationId { get; set; }
}
