using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using DbUp.SqlServer;
using DbUp.Support;

namespace TGC.DatabaseMigration.DBUpWrapper
{
    class CustomJournal : TableJournal
    {
        public CustomJournal(Func<IConnectionManager> connectionManager, Func<IUpgradeLog> logger, string schema, string table)
            : base(connectionManager, logger, new SqlServerObjectParser(), schema, table)
        {
        }

        protected override string CreateSchemaTableSql(string quotedPrimaryKeyName)
        {
            return
                $@"create table {FqSchemaTableName} (
                    [Id] int identity(1,1) not null constraint {quotedPrimaryKeyName} primary key,
                    [ScriptName] nvarchar(255) not null,
                    [MigrationHash] AS CHECKSUM([ScriptName]),
                    [Applied] datetime not null
                )";
        }

        protected override string GetInsertJournalEntrySql(string scriptName, string applied)
        {
            return $"INSERT INTO {FqSchemaTableName} (ScriptName, Applied) VALUES ({@scriptName}, {@applied})";
        }

        protected override string GetJournalEntriesSql()
        {
            return $"select [ScriptName] from {FqSchemaTableName} order by [ScriptName]";
        }
    }
}
