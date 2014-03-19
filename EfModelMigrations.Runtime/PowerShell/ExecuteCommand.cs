using EfModelMigrations.Commands;
using EfModelMigrations.Exceptions;
using EfModelMigrations.Infrastructure.Generators;
using EfModelMigrations.Runtime.Infrastructure.Runners;
using EfModelMigrations.Runtime.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EfModelMigrations.Runtime.Extensions;
using System.IO;
using EfModelMigrations.Runtime.Infrastructure.ModelChanges;

namespace EfModelMigrations.Runtime.PowerShell
{
    internal class ExecuteCommand : PowerShellCommand
    {
        private string commandFullName;
        private object[] parameters;
        

        public ExecuteCommand(string commandFullName, object[] parameters) : base()
        {
            this.commandFullName = commandFullName;
            this.parameters = parameters;

            Execute();
        }

        protected override void ExecuteCore()
        {
            //TODO: Fail if model migrations not enabled

            if (string.IsNullOrEmpty(commandFullName))
            {
                throw new ModelMigrationsException(Resources.CommandNameNotSpecified);
            }

            GeneratedModelMigration migration = null;
            using(var facade = CreateFacade())
            {
                migration = facade.GenerateMigration(commandFullName, parameters);
            }

            if (migration != null)
            {
                Project.AddContentToProject(Path.Combine(migration.MigrationDirectory, migration.MigrationId + ".cs"), migration.SourceCode);
            }
        }
    }
}
