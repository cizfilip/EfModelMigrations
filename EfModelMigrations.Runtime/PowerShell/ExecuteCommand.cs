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
        private object[] parameters;
        

        public ExecuteCommand(string commandFullName, object[] parameters) : base()
        {
            Check.NotEmpty(commandFullName, "commandFullName");

            this.commandFullName = commandFullName;
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

            WriteLine(string.Format("Scaffolding migration from command {0} ...", commandFullName));
            GeneratedModelMigration migration = null;
            using(var facade = CreateFacade())
            {
                migration = facade.GenerateMigration(commandFullName, parameters);
            }

            if (migration != null)
            {
                var migrationPath = Path.Combine(migration.MigrationDirectory, migration.MigrationId + ".cs");
                Project.AddContentToProject(migrationPath, migration.SourceCode);
                WriteLine(string.Format("Migration {0} was succesfully scaffolded.", migration.MigrationId));

                OpenFile(Path.Combine(Project.GetProjectDir(), migrationPath));
            }
        }
    }
}
