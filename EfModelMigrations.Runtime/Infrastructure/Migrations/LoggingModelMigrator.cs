using EfModelMigrations.Configuration;
using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.EntityFramework;
using EfModelMigrations.Resources;
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
                logger.Info(Strings.LoggingModelMigrator_NoMigrations);
            }
            else
            {
                logger.Info(Strings.LoggingModelMigrator_MigrateList(GetDirection(isRevert), string.Join(", ", migrationIds)));
            }

            base.Migrate(migrationIds, isRevert, force);
        }

        internal override void MigrateOne(string migrationId, bool isRevert, bool force)
        {
            logger.Info(Strings.LoggingModelMigrator_MigrateOne(GetDirection(isRevert), migrationId));
            logger.Info(Strings.LoggingModelMigrator_ApplyingModelChanges);

            base.MigrateOne(migrationId, isRevert, force);

            logger.Info(Strings.LoggingModelMigrator_MigrateSuccess(migrationId, 
                GetDirection(isRevert, pastTense: true, lowerCase: true)));
        }

        internal override ScaffoldedMigration GenerateDbMigration(IEnumerable<MigrationOperation> operations, string dbMigrationName)
        {
            logger.Info(Strings.LoggingModelMigrator_GeneratingDbMigration);
            return base.GenerateDbMigration(operations, dbMigrationName);
        }

        internal override void UpdateDatabase(string dbMigrationId)
        {
            logger.Info(Strings.LoggingModelMigrator_UpdatingDb);
            base.UpdateDatabase(dbMigrationId);
        }


        private string GetDirection(bool isRevert, bool pastTense = false, bool lowerCase = false)
        {
            string returnValue;
            if (pastTense)
            {
                returnValue = isRevert ? "Reverted" : "Applied";
            }
            else
            {
                returnValue = isRevert ? "Reverting" : "Applying";
            }

            if (lowerCase)
            {
                returnValue = returnValue.ToLower();
            }

            return returnValue;
        }
    }
}
