using System;

namespace EfModelMigrations.Infrastructure.Generators
{
    [Serializable]
    public class GeneratedModelMigration
    {
        public string MigrationId { get; private set; }
        public string MigrationDirectory { get; private set; }
        public string SourceCode { get; private set; }

        public GeneratedModelMigration(string migrationId, string migrationDirectory, string sourceCode)
        {
            Check.NotEmpty(migrationId, "migrationId");
            Check.NotEmpty(migrationDirectory, "migrationDirectory");
            Check.NotEmpty(sourceCode, "sourceCode");

            this.MigrationId = migrationId;
            this.MigrationDirectory = migrationDirectory;
            this.SourceCode = sourceCode;
        }
    }
}
