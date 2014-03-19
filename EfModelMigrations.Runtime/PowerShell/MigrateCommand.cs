using EfModelMigrations.Runtime.Infrastructure.Migrations;
using EfModelMigrations.Runtime.Infrastructure.Runners.TypeFinders;
using EfModelMigrations.Runtime.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Runtime.PowerShell
{
    class MigrateCommand : PowerShellCommand
    {
        private string targetModelMigration;
        private bool force;

        public MigrateCommand(string targetModelMigration, bool force) : base()
        {
            this.targetModelMigration = targetModelMigration;
            this.force = force;

            Execute();
        }

        protected override void ExecuteCore()
        {
            using(var facade = CreateFacade())
            {
                facade.Migrate(targetModelMigration, force);
            }
        }
    }
}
