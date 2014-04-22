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
using EfModelMigrations.Infrastructure;
using EfModelMigrations.Runtime.Infrastructure.ModelChanges.Helpers;

namespace EfModelMigrations.Runtime.PowerShell
{
    internal class ExecuteCommand : PowerShellCommand
    {
        private string commandFullName;
        private string migrationName;
        private bool rescaffold;
        private object[] parameters;
        

        public ExecuteCommand(string commandFullName, bool rescaffold, string migrationName, object[] parameters) : base()
        {
            Check.NotEmpty(commandFullName, "commandFullName");

            this.commandFullName = commandFullName;
            this.migrationName = migrationName;
            this.parameters = parameters;
            this.rescaffold = rescaffold;

            Execute();
        }

        protected override void ExecuteCore()
        {
            if (string.IsNullOrEmpty(commandFullName))
            {
                throw new ModelMigrationsException(Strings.CommandNameNotSpecified);
            }

            using (var facade = CreateFacade())
            {
                //Fail if model migrations not enabled
                string configurationType = facade.FindModelMigrationsConfiguration();
                if (string.IsNullOrEmpty(configurationType))
                {
                    WriteLine(Strings.ModelMigrationsNotEnabled);
                    return;
                }

                if (rescaffold)
                {
                    WriteLine(Strings.ExecuteCommand_RescaffoldingMigration(commandFullName));
                }
                else
                {
                    WriteLine(Strings.ExecuteCommand_ScaffoldingMigration(commandFullName));
                }
                
                GeneratedModelMigration migration = null;
                migration = facade.GenerateMigration(commandFullName, rescaffold, migrationName, parameters);
                if (migration != null)
                {
                    var migrationPath = Path.Combine(migration.MigrationDirectory, migration.MigrationId + ".cs");

                    if (rescaffold)
                    {
                        var codeClassFinder = new CodeClassFinder(Project);
                        var migrationClass = codeClassFinder.FindCodeClassFromFullName(migration.MigrationClassFullName);

                        var upMethod = migrationClass.FindMethod("Up");
                        var downMethod = migrationClass.FindMethod("Down");

                        upMethod.InsertAtEnd(migration.UpMethodSourceCode);
                        downMethod.InsertAtStart(migration.DownMethodSourceCode);

                        WriteLine(Strings.ExecuteCommand_MigrationRescaffolded(migration.MigrationId));
                    }
                    else
                    {
                        Project.AddContentToProject(migrationPath, migration.SourceCode);
                        WriteLine(Strings.ExecuteCommand_MigrationScaffolded(migration.MigrationId));
                    }

                    OpenFile(Path.Combine(Project.GetProjectDir(), migrationPath));
                }
            }
        }
    }
}
