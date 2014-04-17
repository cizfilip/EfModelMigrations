using EfModelMigrations.Configuration;
using EfModelMigrations.Exceptions;
using EfModelMigrations.Extensions;
using EfModelMigrations.Infrastructure;
using EfModelMigrations.Infrastructure.EntityFramework;
using EfModelMigrations.Operations;
using EfModelMigrations.Runtime.Infrastructure.ModelChanges;
using EfModelMigrations.Resources;
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
        private Func<string, IClassModelProvider> classModelProviderFactory;
        private IModelChangesExecutor modelChangesExecutor;
        private ModelMigrationsConfigurationBase configuration;
        private ModelMigrationsLocator locator;
        private VsProjectBuilder projectBuilder;
        private DbMigrationWriter dbMigrationWriter;
        private string migrationProjectPath;

        public ModelMigrator(HistoryTracker historyTracker,
            ModelMigratorHelper migratorHelper,
            Func<string, IClassModelProvider> classModelProviderFactory,
            IModelChangesExecutor modelChangesExecutor,
            ModelMigrationsConfigurationBase configuration,
            VsProjectBuilder projectBuilder,
            DbMigrationWriter dbMigrationWriter,
            string migrationProjectPath)
        {
            this.historyTracker = historyTracker;
            this.migratorHelper = migratorHelper;
            this.classModelProviderFactory = classModelProviderFactory;
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

                historyTracker.Reset();
            }
        }

        internal virtual void MigrateOne(string migrationId, bool isRevert, bool force)
        {
            string oldEdmxModel;
            ScaffoldedMigration scaffoldedMigration = null;

            try
            {
                var migration = GetMigration(migrationId);
                var transformations = GetModelTransformations(migration, isRevert);

                if (transformations.Where(t => t.IsDestructiveChange).Any() && !force)
                {
                    throw new ModelMigrationsException(Strings.DataInDbMayBeLost(migration.Name));
                }

                var dbMigrationOperations = new List<MigrationOperation>();

                oldEdmxModel = migratorHelper.GetEdmxModel();
                //proccess transformations
                foreach (var transformation in transformations)
                {
                    var modelProvider = classModelProviderFactory(oldEdmxModel);

                    //Verify preconditions
                    VerifyPreconsitions(transformation, modelProvider);

                    //apply model changes
                    ApplyModelChanges(transformation, modelProvider);

                    //build & get new model
                    projectBuilder.BuildModelProject();
                    string newEdmxModel = migratorHelper.GetEdmxModel();

                    //get db migration operations & append it to list
                    var dbOperations = GetDbMigrationOperations(transformation, oldEdmxModel, newEdmxModel);
                    dbMigrationOperations.AddRange(dbOperations);

                    oldEdmxModel = newEdmxModel;
                }
                
                //generate db migration    
                scaffoldedMigration = GenerateDbMigration(dbMigrationOperations, GetDbMigrationName(migration, isRevert));
                //write migration - edmxModel is supplied because ef MigrationScaffolder see only old model before migrating
                dbMigrationWriter.Write(scaffoldedMigration, oldEdmxModel);

                //build project before updating DB
                projectBuilder.BuildModelProject();

                //update DB
                UpdateDatabase(scaffoldedMigration.MigrationId);

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
                throw new ModelMigrationsException(Strings.ApplyMigrationError, e);
            }
        }

        internal virtual void VerifyPreconsitions(ModelTransformation transformation, IClassModelProvider modelProvider)
        {
            var preconditions = transformation.GetPreconditions();

            foreach (var precondition in preconditions)
            {
                var result = precondition.Verify(modelProvider);
                if (!result.Success)
                {
                    string transformationName = transformation.GetType().Name.RemoveFromEnd("Transformation");
                    throw new ModelTransformationValidationException(
                        string.Concat(Strings.PreconditionFailed(transformationName), 
                        Environment.NewLine,
                        result.Message)
                    );
                }
            }
        }

       
        internal virtual void ApplyModelChanges(ModelTransformation transformation, IClassModelProvider modelProvider)
        {
            IEnumerable<IModelChangeOperation> operations = transformation.GetModelChangeOperations(modelProvider);
            modelChangesExecutor.Execute(operations);
        }

        internal virtual IEnumerable<MigrationOperation> GetDbMigrationOperations(ModelTransformation transformation, string oldEdmxModel, string newEdmxModel)
        {
            var operationBuilder = new DbMigrationOperationBuilder(new EfModel(oldEdmxModel), new EfModel(newEdmxModel));

            return transformation.GetDbMigrationOperations(operationBuilder).ToList();
        }

        internal virtual ScaffoldedMigration GenerateDbMigration(IEnumerable<MigrationOperation> operations, string dbMigrationName)
        {
            ((ExtendedCSharpMigrationCodeGenerator)configuration.DbMigrationsConfiguration.CodeGenerator).NewOperations = operations;

            MigrationScaffolder scaffolder = new MigrationScaffolder(configuration.DbMigrationsConfiguration);

            var scaffoldedMigration = scaffolder.Scaffold(dbMigrationName, ignoreChanges: true);

            return scaffoldedMigration;
        }

        internal virtual void UpdateDatabase(string dbMigrationId)
        {
            migratorHelper.UpdateDatabase(dbMigrationId);
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

    }
}
