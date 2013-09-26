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

namespace EfModelMigrations.Runtime.PowerShell
{
    internal class ModelCommand : PowerShellCommand
    {
        private string commandName;
        

        public ModelCommand(string commandName, string[] parameters) : base(parameters)
        {
            this.commandName = commandName;

            Execute();
        }

        protected override void ExecuteCore()
        {
            //TODO: Fail if model migrations not enabled

            if (string.IsNullOrEmpty(commandName))
            {
                throw new ModelMigrationsException(Resources.CommandNameNotSpecified);
            }


            GeneratedModelMigration migration = null;
            using (var executor = CreateExecutor())
            {
                migration = executor.ExecuteRunner<GeneratedModelMigration>(new GenerateMigrationFromCommandRunner() 
                        { 
                            CommandName = this.commandName,
                            Parameters = base.Parameters
                        });
            }

            if (migration != null)
            {
                Project.AddContentToProject(Path.Combine(migration.MigrationDirectory, migration.MigrationId + ".cs"), migration.SourceCode);
            }




            //string fullCommandName = commandName + "Command";
            
            //var commandType = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
            //                   from type in assembly.GetTypes()
            //                   where typeof(ModelMigrationCommand).IsAssignableFrom(type) && string.Equals(type.Name, fullCommandName, StringComparison.OrdinalIgnoreCase)
            //                   select type).Single();

            //var command = Activator.CreateInstance(commandType, new object[] { Parameters }) as ModelMigrationCommand;

            //WriteLine("Command first param:");
            //WriteLine(command.Parameters.First());
        }
    }
}
