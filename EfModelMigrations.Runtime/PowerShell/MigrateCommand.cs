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
        private string targetMigrationId;

        public MigrateCommand(string[] parameters)
            : base(parameters)
        {
            if (parameters.Length > 0)
            {
                this.targetMigrationId = parameters[0];
            }
            
            Execute();
        }

        protected override void ExecuteCore()
        {
            MigrationsToApplyOrRevertResult result;
            using (var executor = CreateExecutor())
            {
                result = executor.ExecuteRunner<MigrationsToApplyOrRevertResult>(new FindMigrationsToApplyOrRevert()
                {
                    TargetMigrationId = targetMigrationId
                });
            }

            if (result != null)
            {
                if (result.ModelMigrationsIds.Any())
                {
                    var migrator = new ModelMigrator(CreateExecutor);
                    if (result.IsRevert)
                    {
                        migrator.RevertMigrations(result.ModelMigrationsIds);
                    }
                    else
                    {
                        migrator.ApplyMigrations(result.ModelMigrationsIds);
                    }
                }
                else
                {
                    WriteLine(Resources.NoMigrationsToMigrate);
                }
            }
        }
    }
}
