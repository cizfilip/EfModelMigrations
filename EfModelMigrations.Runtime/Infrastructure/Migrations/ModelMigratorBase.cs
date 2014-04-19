using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.EntityFramework;
using EfModelMigrations.Runtime.Infrastructure.ModelChanges;
using EfModelMigrations.Transformations;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Design;
using System.Data.Entity.Migrations.Model;

namespace EfModelMigrations.Runtime.Infrastructure.Migrations
{
    internal abstract class ModelMigratorBase
    {
        private ModelMigratorBase innerMigrator;

        public ModelMigratorBase(ModelMigratorBase innerMigrator)
        {
            if (innerMigrator == null)
            {
                this.innerMigrator = this;
            }
            else
            {
                this.innerMigrator = innerMigrator;

                var nextMigrator = innerMigrator;

                while (nextMigrator.innerMigrator != innerMigrator)
                {
                    nextMigrator = nextMigrator.innerMigrator;
                }

                nextMigrator.innerMigrator = this;
            }
        }

        public virtual void Migrate(string targetMigration, bool force)
        {
            innerMigrator.Migrate(targetMigration, force);
        }

        internal virtual void Migrate(IEnumerable<string> migrationIds, bool isRevert, bool force)
        {
            innerMigrator.Migrate(migrationIds, isRevert, force);
        }

        internal virtual void MigrateOne(string migrationId, bool isRevert, bool force)
        {
            innerMigrator.MigrateOne(migrationId, isRevert, force);
        }

        internal virtual void VerifyPreconsitions(ModelTransformation transformation, IClassModelProvider modelProvider)
        {
            innerMigrator.VerifyPreconsitions(transformation, modelProvider);
        }

        internal virtual void ApplyModelChanges(ModelTransformation transformation, IClassModelProvider modelProvider)
        {
            innerMigrator.ApplyModelChanges(transformation, modelProvider);
        }

        internal virtual IEnumerable<MigrationOperation> GetDbMigrationOperations(ModelTransformation transformation, EfModelMetadata oldEfModelMetadata, EfModelMetadata newEfModelMetadata)
        {
            return innerMigrator.GetDbMigrationOperations(transformation, oldEfModelMetadata, newEfModelMetadata);
        }

        internal virtual ScaffoldedMigration GenerateDbMigration(IEnumerable<MigrationOperation> operations, string dbMigrationName)
        {
            return innerMigrator.GenerateDbMigration(operations, dbMigrationName);
        }

        internal virtual void UpdateDatabase(string dbMigrationId)
        {
            innerMigrator.UpdateDatabase(dbMigrationId);
        }

        internal virtual void RollbackModelState(HistoryTracker historyTracker, ScaffoldedMigration scaffoldedMigration)
        {
            innerMigrator.RollbackModelState(historyTracker, scaffoldedMigration);
        }
    }
}
