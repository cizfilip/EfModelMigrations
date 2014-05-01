using System;

namespace EfModelMigrations.Infrastructure.Generators
{
    [Serializable]
    public class GeneratedModelMigration
    {
        public string MigrationId { get; private set; }
        public string MigrationClassFullName { get; private set; }
        public string MigrationDirectory { get; private set; }
        public string UpMethodSourceCode { get; private set; }
        public string DownMethodSourceCode { get; private set; }
        public string SourceCode { get; private set; }

        public GeneratedModelMigration(string migrationId,
            string migrationClassFullName,
            string migrationDirectory, 
            string sourceCode, 
            string upMethodSourceCode, 
            string downMethodSourceCode)
        {
            Check.NotEmpty(migrationId, "migrationId");
            Check.NotEmpty(migrationClassFullName, "migrationClassFullName");
            Check.NotEmpty(migrationDirectory, "migrationDirectory");
            Check.NotEmpty(sourceCode, "sourceCode");
            Check.NotNull(upMethodSourceCode, "upMethodSourceCode");
            Check.NotNull(downMethodSourceCode, "downMethodSourceCode");

            this.MigrationId = migrationId;
            this.MigrationClassFullName = migrationClassFullName;
            this.MigrationDirectory = migrationDirectory;
            this.SourceCode = sourceCode;
            this.UpMethodSourceCode = upMethodSourceCode;
            this.DownMethodSourceCode = downMethodSourceCode;
        }
    }
}
