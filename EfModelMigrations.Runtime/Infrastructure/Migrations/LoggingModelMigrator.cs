using EfModelMigrations.Configuration;
using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.EntityFramework;
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
    internal class LoggingModelMigrator : ModelMigratorBase
    {
        private RunnerLogger logger;

        public LoggingModelMigrator(ModelMigrator modelMigrator, RunnerLogger logger) : base(modelMigrator)
        {
            this.logger = logger;
        }

        internal override void Migrate(IEnumerable<string> migrationIds, bool isRevert, bool force)
        {
            if(!migrationIds.Any())
            {
                logger.Info("No migrations to apply or revert.");
            }
            else
            {
                string direction = isRevert ? "Reverting" : "Applying";

                logger.Info(string.Format("{0} migrations: [{1}].", direction, string.Join(", ", migrationIds)));
            }

            base.Migrate(migrationIds, isRevert, force);
        }

        internal override void MigrateOne(string migrationId, bool isRevert, bool force)
        {
            string direction = isRevert ? "Reverting" : "Applying";
            

            logger.Info(string.Format("{0} migration: {1}.", direction, migrationId));
            logger.Info("Applying model changes...");

            base.MigrateOne(migrationId, isRevert, force);

            string direction2 = isRevert ? "reverted" : "applied";
            logger.Info(string.Format("Migration {0} was succesfully {1}.", migrationId, direction2));
        }

        internal override ScaffoldedMigration GenerateDbMigration(IEnumerable<MigrationOperation> operations, string dbMigrationName)
        {
            logger.Info("Generating Db migration...");
            return base.GenerateDbMigration(operations, dbMigrationName);
        }

        internal override void UpdateDatabase(string dbMigrationId)
        {
            logger.Info("Updating database...");
            base.UpdateDatabase(dbMigrationId);
        }


    }
}
