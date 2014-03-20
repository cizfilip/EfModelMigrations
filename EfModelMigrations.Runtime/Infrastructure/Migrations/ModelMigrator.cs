using EfModelMigrations.Configuration;
using EfModelMigrations.Exceptions;
using EfModelMigrations.Extensions;
using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.EntityFramework;
using EfModelMigrations.Operations;
using EfModelMigrations.Runtime.Infrastructure.ModelChanges;
using EfModelMigrations.Runtime.Properties;
using EfModelMigrations.Transformations;
using EnvDTE;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.Design;
using System.Data.Entity.Migrations.Model;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace EfModelMigrations.Runtime.Infrastructure.Migrations
{
    internal class ModelMigrator
    {
        private HistoryTracker historyTracker;
        private ModelMigratorHelper migratorHelper;
        private IClassModelProvider classModelProvider;
        private IModelChangesExecutor modelChangesExecutor;
        private ModelMigrationsConfigurationBase configuration;
        private ModelMigrationsLocator locator;
        private VsProjectBuilder projectBuilder;
        private DbMigrationWriter dbMigrationWriter;
        private string migrationProjectPath;

        public ModelMigrator(HistoryTracker historyTracker, 
            ModelMigratorHelper migratorHelper,
            IClassModelProvider classModelProvider, 
            IModelChangesExecutor modelChangesExecutor,
            ModelMigrationsConfigurationBase configuration,
            VsProjectBuilder projectBuilder,
            DbMigrationWriter dbMigrationWriter,
            string migrationProjectPath)
        {
            this.historyTracker = historyTracker;
            this.migratorHelper = migratorHelper;
            this.classModelProvider = classModelProvider;
            this.modelChangesExecutor = modelChangesExecutor;
            this.configuration = configuration;
            this.locator = new ModelMigrationsLocator(configuration);
            this.projectBuilder = projectBuilder;
            this.dbMigrationWriter = dbMigrationWriter;
            this.migrationProjectPath = migrationProjectPath;
        }


        public virtual void Migrate(string targetMigration, bool force)
        {
            bool isRevert;
            var migrations = locator.FindModelMigrationsToApplyOrRevert(targetMigration, out isRevert);
            Migrate(migrations, isRevert, force);
        }


        internal virtual void Migrate(IEnumerable<string> migrationIds, bool isRevert, bool force)
        {
            foreach (var migrationId in migrationIds)
            {
                MigrateOne(migrationId, isRevert, force);
                //TODO: pokud prijjimam history tracker tak po kazde migraci to na nem chce volat metodu Reset
                historyTracker.Reset();
            }
        }

        internal virtual void MigrateOne(string migrationId, bool isRevert, bool force)
        {
            string oldEdmxModel;
            ScaffoldedMigration scaffoldedMigration = null;

            try
            {
                oldEdmxModel = migratorHelper.GetEdmxModel();

                var migration = GetMigration(migrationId);
                var transformations = GetModelTransformations(migration, isRevert);

                if (transformations.Where(t => t.IsDestructiveChange).Any() && !force)
                {
                    throw new ModelMigrationsException(string.Format("Some operations in migration {0} may cause data loss in database! If you really want to execute this migration rerun the migrate command with -Force parameter.", migration.Name)); //TODO: string do resourcu
                }

                //apply model changes
                ApplyModelChanges(transformations);

                //build project and get new edmx model
                projectBuilder.BuildModelProject();
                string newEdmxModel = migratorHelper.GetEdmxModel();

                //generate db migration    
                scaffoldedMigration = GenerateDbMigration(transformations, oldEdmxModel, newEdmxModel, GetDbMigrationName(migration, isRevert));
                //write migration
                dbMigrationWriter.Write(scaffoldedMigration);

                //build project
                projectBuilder.BuildModelProject();

                //update DB
                migratorHelper.UpdateDatabase(scaffoldedMigration.MigrationId);
               
                //update applied migration in configuration 
                UpdateConfiguration(migrationId, scaffoldedMigration.MigrationId, isRevert);                
            }
            //TODO: Pokud dojde k vyjimce v restore na historyTrackeru tak se vyhodi jen ta a my zapomeneme duvod proc jsme vubec restorovali
            //idealni vypis:
            // 1) message vyjimky ktera zpusobila nutnost restoru!
            // 2) pokud se restore provedl ok vypsat: vsechny zmeny pred chybou byli vraceny, pokud je vyjimka i v restoru napsat ze je to uplne v haji a at si uzivatel poradi sam :)
            catch (ModelMigrationsException)
            {
                RollbackModelState(historyTracker, scaffoldedMigration);
                throw;
            }
            catch (Exception e)
            {
                RollbackModelState(historyTracker, scaffoldedMigration);
                throw new ModelMigrationsException(Resources.ApplyMigrationError, e);
            }
        }

        internal virtual void ApplyModelChanges(IEnumerable<ModelTransformation> transformations)
        {
            IEnumerable<IModelChangeOperation> operations = transformations.SelectMany(t => t.GetModelChangeOperations(classModelProvider));
            modelChangesExecutor.Execute(operations);   
        }

        internal virtual ScaffoldedMigration GenerateDbMigration(IEnumerable<ModelTransformation> transformations, string oldEdmxModel, string newEdmxModel, string dbMigrationName)
        {
            var operationBuilder = new DbMigrationOperationBuilder(configuration.ModelNamespace, LoadEdmxFromString(oldEdmxModel), LoadEdmxFromString(newEdmxModel));

            IEnumerable<MigrationOperation> dbMigrationOperations = transformations.SelectMany(t => t.GetDbMigrationOperations(operationBuilder));

            ((ExtendedCSharpMigrationCodeGenerator)configuration.DbMigrationsConfiguration.CodeGenerator).NewOperations = dbMigrationOperations;

            MigrationScaffolder scaffolder = new MigrationScaffolder(configuration.DbMigrationsConfiguration);

            var scaffoldedMigration = scaffolder.Scaffold(dbMigrationName, ignoreChanges: true);

            return scaffoldedMigration;
        }

        internal virtual void UpdateConfiguration(string appliedModelMigrationId, string appliedDbMigrationId, bool isRevert)
        {
            //TODO: updatovat na projekt kde jsou migrace ne modelProject - az budu podporovat vicero projektu
            string configurationResourcePath = Path.Combine(migrationProjectPath, configuration.ModelMigrationsDirectory, configuration.GetType().Name + ".resx");
            var updater = new ModelMigrationsConfigurationUpdater(configurationResourcePath);

            if (!isRevert)
            {
                updater.AddAppliedMigration(appliedModelMigrationId, appliedDbMigrationId);
            }
            else
            {
                updater.RemoveLastAppliedMigration(appliedDbMigrationId);
            }
        }

        internal virtual ModelMigration GetMigration(string migrationId)
        {
            var modelMigrationType = locator.FindModelMigration(migrationId);
            return modelMigrationType.CreateInstance<ModelMigration>();
        }

        internal virtual string GetDbMigrationName(ModelMigration migration, bool isRevert)
        {
            string dbMigrationName = migration.Name;
            if (isRevert)
            {
                dbMigrationName = "Revert" + dbMigrationName;
            }
            return dbMigrationName;
        }

        internal virtual IEnumerable<ModelTransformation> GetModelTransformations(ModelMigration migration, bool isRevert)
        {
            migration.Reset();
            if (isRevert)
            {
                migration.Down();
            }
            else
            {
                migration.Up();
            }

            return migration.Transformations;
        }

        internal virtual void RollbackModelState(HistoryTracker historyTracker, ScaffoldedMigration scaffoldedMigration)
        {
            historyTracker.Restore();

            //Ensure db migration is removed
            if (scaffoldedMigration != null)
            {
                dbMigrationWriter.RemoveMigration(scaffoldedMigration);
            }
        }


        private XDocument LoadEdmxFromString(string edmx)
        {
            return XDocument.Parse(edmx);
        }
    }
}
