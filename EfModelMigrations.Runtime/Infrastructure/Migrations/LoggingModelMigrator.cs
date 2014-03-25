using EfModelMigrations.Configuration;
using EfModelMigrations.Infrastructure;
using EfModelMigrations.Runtime.Infrastructure.ModelChanges;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Design;
using System.Data.Entity.Migrations.Model;
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
            Func<string, IClassModelProvider> classModelProviderFactory, 
            IModelChangesExecutor modelChangesExecutor,
            ModelMigrationsConfigurationBase configuration,
            VsProjectBuilder projectBuilder,
            DbMigrationWriter dbMigrationWriter,
            string migrationProjectPath)
         
        :base(
            historyTracker,
            migratorHelper,
            classModelProviderFactory,
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
            Logger.Info("Applying model changes...");

            base.MigrateOne(migrationId, isRevert, force);

            string direction2 = isRevert ? "reverted" : "applied";
            Logger.Info(string.Format("Migration {0} was succesfully {1}.", migrationId, direction2));
        }

        internal virtual ScaffoldedMigration GenerateDbMigration(IEnumerable<MigrationOperation> operations, string dbMigrationName)
        {
            Logger.Info("Generating Db migration...");
            return base.GenerateDbMigration(operations, dbMigrationName);
        }

        internal override void UpdateDatabase(string dbMigrationId)
        {
            Logger.Info("Updating database...");
            base.UpdateDatabase(dbMigrationId);
        }

    }
}
