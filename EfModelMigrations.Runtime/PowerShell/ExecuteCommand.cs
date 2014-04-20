using EfModelMigrations.Commands;
using EfModelMigrations.Exceptions;
using EfModelMigrations.Infrastructure.Generators;
using EfModelMigrations.Runtime.Infrastructure.Runners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EfModelMigrations.Runtime.Extensions;
using System.IO;
using EfModelMigrations.Runtime.Infrastructure.ModelChanges;
using EfModelMigrations.Resources;

namespace EfModelMigrations.Runtime.PowerShell
{
    internal class ExecuteCommand : PowerShellCommand
    {
        private string commandFullName;
        private string migrationName;
        private object[] parameters;
        

        public ExecuteCommand(string commandFullName, string migrationName, object[] parameters) : base()
        {
            Check.NotEmpty(commandFullName, "commandFullName");

            this.commandFullName = commandFullName;
            this.migrationName = migrationName;
            this.parameters = parameters;

            Execute();
        }

        protected override void ExecuteCore()
        {
            //TODO: Fail if model migrations not enabled

            if (string.IsNullOrEmpty(commandFullName))
            {
                throw new ModelMigrationsException(Strings.CommandNameNotSpecified);
            }

            WriteLine(Strings.ExecuteCommand_ScaffoldingMigration(commandFullName));
            GeneratedModelMigration migration = null;
            using(var facade = CreateFacade())
            {
                migration = facade.GenerateMigration(commandFullName, migrationName, parameters);
            }

            if (migration != null)
            {
                var migrationPath = Path.Combine(migration.MigrationDirectory, migration.MigrationId + ".cs");
                Project.AddContentToProject(migrationPath, migration.SourceCode);
                WriteLine(Strings.ExecuteCommand_MigrationScaffolded(migration.MigrationId));

                OpenFile(Path.Combine(Project.GetProjectDir(), migrationPath));
            }
        }
    }
}
