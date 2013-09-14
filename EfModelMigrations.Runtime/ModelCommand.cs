using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Runtime
{
    internal class ModelCommand : PowerShellCommand
    {
        public ModelCommand(string commandName, string[] parameters)
        {
            WriteLine(commandName);

            Execute();
        }

        protected override void ExecuteCore()
        {
            WriteLine("Jupi jsme tam");
        }
    }
}
