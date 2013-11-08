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
        private string targetModelMigrationId;

        public MigrateCommand(string targetModelMigrationId) : base()
        {
            this.targetModelMigrationId = targetModelMigrationId;

            Execute();
        }

        protected override void ExecuteCore()
        {
            MigrationsToApplyOrRevertResult result;
            using (var executor = CreateExecutor())
            {
                result = executor.ExecuteRunner<MigrationsToApplyOrRevertResult>(new FindMigrationsToApplyOrRevert()
                {
                    TargetMigrationId = targetModelMigrationId
                });
            }


            if (result.ModelMigrationsIds.Any())
            {
                var migrator = new ModelMigrator(Project, CreateExecutor);
                migrator.Migrate(result.ModelMigrationsIds, result.IsRevert);
            }
            else
            {
                WriteLine(Resources.NoMigrationsToApplyOrRevert);
            }

        }
    }
}
