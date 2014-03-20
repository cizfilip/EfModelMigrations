using EfModelMigrations.Configuration;
using EfModelMigrations.Infrastructure;
using EfModelMigrations.Runtime.Infrastructure.ModelChanges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Runtime.Infrastructure.Migrations
{
    //TODO: hlasky do resourcu
    internal class LoggingModelMigrator : ModelMigrator
    {
        public RunnerLogger Logger { get; set; }

        public LoggingModelMigrator(HistoryTracker historyTracker, 
            ModelMigratorHelper migratorHelper,
            IClassModelProvider classModelProvider, 
            IModelChangesExecutor modelChangesExecutor,
            ModelMigrationsConfigurationBase configuration,
            VsProjectBuilder projectBuilder,
            DbMigrationWriter dbMigrationWriter,
            string migrationProjectPath)
         
        :base(
            historyTracker,
            migratorHelper,
            classModelProvider,
            modelChangesExecutor,
            configuration,
            projectBuilder,
            dbMigrationWriter,
            migrationProjectPath)
        {
        }


        internal override void Migrate(IEnumerable<string> migrationIds, bool isRevert, bool force)
        {
            if(!migrationIds.Any())
            {
                Logger.Info("No migrations to apply or revert.");
            }
            else
            {
                string direction = isRevert ? "Reverting" : "Applying";

                Logger.Info(string.Format("{0} migrations: [{1}].", direction, string.Join(", ", migrationIds)));
            }

            base.Migrate(migrationIds, isRevert, force);
        }

        internal override void MigrateOne(string migrationId, bool isRevert, bool force)
        {
            string direction = isRevert ? "Reverting" : "Applying";

            Logger.Info(string.Format("{0} migration: {1}.", direction, migrationId));

            base.MigrateOne(migrationId, isRevert, force);
        }

    }
}
