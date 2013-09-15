using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Runtime
{
    class EnableCommand : PowerShellCommand
    {
        public EnableCommand(string[] parameters) : base(parameters)
        {
            Execute();
        }

        protected override void ExecuteCore()
        {
            //TODO: Create Configuration object, DbContext, and call EF cmd Enable-Migrations
        }
    }
}
