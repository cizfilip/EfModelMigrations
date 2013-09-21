using EfModelMigrations.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            string fullCommandName = commandName + "Command";
            
            var commandType = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                               from type in assembly.GetTypes()
                               where typeof(ModelMigrationCommand).IsAssignableFrom(type) && string.Equals(type.Name, fullCommandName, StringComparison.OrdinalIgnoreCase)
                               select type).Single();

            var command = Activator.CreateInstance(commandType, new object[] { Parameters }) as ModelMigrationCommand;

            WriteLine("Command first param:");
            WriteLine(command.Parameters.First());
        }
    }
}
